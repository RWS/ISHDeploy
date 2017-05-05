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
﻿
namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Performs different kinds of operations with text file
    /// </summary>
    public interface ITextConfigManager
    {
        /// <summary>
        /// Comments block of text file between two <paramref name="searchPattern"/> comments
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="searchPattern">Comment pattern that is searched for</param>
        void CommentBlock(string filePath, string searchPattern);

        /// <summary>
        /// Replace text that matches the <paramref name="searchPattern"/>
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="searchPattern">The pattern that is searched for</param>
        /// <param name="newValue">The new value</param>
        void TextReplace(string filePath, string searchPattern, string newValue);

        /// <summary>
        /// Uncomments block of text file between two <paramref name="searchPattern"/> comments
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="searchPattern">Comment pattern that is searched for</param>
        void UncommentBlock(string filePath, string searchPattern);
    }
}
