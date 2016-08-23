/*
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using ISHDeploy.Interfaces;
using System;

namespace ISHDeploy.Data.Actions.StringActions
{
    /// <summary>
    /// The action is responsible to create content for index.html file for internal STS authentication.
    /// </summary>
    /// <seealso cref="BaseActionWithResult{TResult}"></seealso>
    public class CreateIndexHTMLAction : BaseActionWithResult<string>
    {
        /// <summary>
        /// The Host name.
        /// </summary>
        private readonly string _lCHost;

        /// <summary>
        /// The application name.
        /// </summary>
        private readonly string _lCWebAppName;

        private readonly string _linkISHCM;
        private readonly string _linkISHWS;
        private readonly string _linkISHSTS;

        private string html = @"<!DOCTYPE html>

<html lang = ""en"" xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <meta charset = ""utf-8"" />
    <title> Knowledge Center SDL support access links</title>
{script}
</head>
<body {onload}>
    <div app = ""ISH"" >
        <h1> Content Manager</h1>
        <p><a href = ""{linkISHCM}"" > Content Manager</a></p>
        <p>Use <b>{linkISHWS}/</b> as the value for SDL Knowledge Center Content Manager web service</p>
    </div>
{divLC}
</body>
</html>";

        private string script = @"
    <script>
        function AccessContentDelivery()
        {
            var cdUrl = ""{cdURL}""
            var xmlHttp = new XMLHttpRequest();
            xmlHttp.open(""GET"", cdUrl, true); // true for asynchronous 
            xmlHttp.send(null);
        }
    </script>
";

        private string divLC = @"
    <div app = ""LC"" >
        <h1> Content Delivery</h1>
        <p><a href = ""{linkCD}"" > Content Delivery</a></p>
    </div>
";
        /// <summary>
        /// Initializes new instance of the <see cref="CreateIndexHTMLAction"/>
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILogger"/></param>
        /// <param name="lCHost">Host name.</param>
        /// <param name="lCWebAppName">Application name.</param>
        /// <param name="returnResult">The delegate that returns content for index.html file.</param>
        public CreateIndexHTMLAction(ILogger logger,
            string linkISHCM,
            string linkISHWS,
            string linkISHSTS,
            string lCHost,
            string lCWebAppName,
            Action<string> returnResult) : base(logger, returnResult)
        {
            this._linkISHCM = linkISHCM;
            this._linkISHWS = linkISHWS;
            this._linkISHSTS = linkISHSTS;
            this._lCHost = lCHost;
            this._lCWebAppName = lCWebAppName;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        protected override string ExecuteWithResult()
        {
            // originally from here: https://confluence.sdl.com/download/attachments/67406928/Prepare-SupportAccess.ps1?version=1&modificationDate=1450257894000&api=v2
            string onload;

            string link = BuildWSFedUrl(new Uri(_linkISHCM));
            html = html.Replace("{linkISHCM}", link);
            html = html.Replace("{linkISHWS}", _linkISHWS);

            if (_lCHost != null)
            {
                // Initially with the CD links
                string cdURL = "https://" + _lCHost + "/" + _lCWebAppName;
                var hostUri = new Uri(cdURL);
                var linkCD = BuildWSFedUrl(hostUri);
                script = script.Replace("{cdURL}", cdURL);
                divLC = divLC.Replace("{linkCD}", linkCD);
                onload = @" onload=""AccessContentDelivery()""";
            }
            else
            {
                // Nullify CD assets
                script = "";
                divLC = "";
                onload = "";
            }

            html = html.Replace("{script}", script);
            html = html.Replace("{divLC}", divLC);
            html = html.Replace("{onload}", onload);

            Logger.WriteHostEmulation($"Generated internal URL:    {link}");
            return html;
        }

        private string BuildWSFedUrl(Uri realm)
        {
            var builder = new UriBuilder(_linkISHSTS);
            builder.Path += "issue/wsfed";
            builder.Query = "wa=wsignin1.0&wtrealm=" + Uri.EscapeDataString(realm.ToString());
            return builder.Uri.AbsoluteUri;
        }
    }
}
