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

using System;
using System.Runtime.InteropServices;
using COMAdmin;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Singleton wrapper for COMAdminCatalog class.
    /// </summary>
    public class COMAdminCatalogWrapperSingleton : IDisposable
    {
        /// <summary>
        /// Singleton private instance
        /// </summary>
        private static readonly Lazy<COMAdminCatalogWrapperSingleton> Singleton = 
            new Lazy<COMAdminCatalogWrapperSingleton>(() => new COMAdminCatalogWrapperSingleton());

        /// <summary>
        /// Returns Singleton instance of the <see cref="COMAdminCatalogWrapperSingleton"/> class.
        /// </summary>
        public static COMAdminCatalogWrapperSingleton Instance => Singleton.Value;

        /// <summary>
        /// The COMAdminCatalog.
        /// </summary>
        private readonly ICOMAdminCatalog _comAdminCatalog;

        /// <summary>
        /// The COM applications.
        /// </summary>
        private ICatalogCollection _comApplications;

        /// <summary>
        /// The COM application instances.
        /// </summary>
        private ICatalogCollection _comApplicationInstances;

        /// <summary>
        /// Initializes a new instance of the <see cref="COMAdminCatalogWrapperSingleton"/> class.
        /// </summary>
        private COMAdminCatalogWrapperSingleton()
        {
            _comAdminCatalog = (ICOMAdminCatalog)Activator.CreateInstance(Type.GetTypeFromProgID("COMAdmin.COMAdminCatalog"));
        }

        /// <summary>
        /// Gets COM applications
        /// </summary>
        /// <returns>COM applications</returns>
        public ICatalogCollection GetApplications()
        {
            if (_comApplications != null)
            {
                lock (_comApplications)
                {
                    if (_comApplications != null)
                    {
                        Dispose(_comApplications);
                    }
                }
            }
            _comApplications = (ICatalogCollection)_comAdminCatalog.GetCollection("Applications");
            _comApplications.Populate();
            return _comApplications;
        }

        /// <summary>
        /// Gets COM applications instances
        /// </summary>
        /// <returns>COM applications</returns>
        public ICatalogCollection GetApplicationInstances()
        {
            if (_comApplicationInstances != null)
            {
                lock (_comApplicationInstances)
                {
                    if (_comApplicationInstances != null)
                    {
                        Dispose(_comApplicationInstances);
                    }
                }
            }
            _comApplicationInstances = (ICatalogCollection)_comAdminCatalog.GetCollection("ApplicationInstances");
            _comApplicationInstances.Populate();
            return _comApplicationInstances;
        }

        /// <summary>
        /// Shutdown COM+ components
        /// </summary>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        public void ShutdownApplication(string comPlusComponentName)
        {
            _comAdminCatalog.ShutdownApplication(comPlusComponentName);
        }

        /// <summary>
        /// Starts COM+ components
        /// </summary>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        public void StartApplication(string comPlusComponentName)
        {
            _comAdminCatalog.StartApplication(comPlusComponentName);
        }

        /// <summary>
        /// Dispose the given object
        /// </summary>
        /// <param name="disposeObject">The object to dispose</param>
        private void Dispose(object disposeObject)
        {
            if (disposeObject != null && disposeObject.GetType().ToString().ToLower() == "system.__comobject")
            {
                Marshal.ReleaseComObject(disposeObject);
                Marshal.FinalReleaseComObject(disposeObject);
                disposeObject = null;
            }
        }

        /// <summary>
        /// Release the COM objects
        /// </summary>
        public void Dispose(bool disposing)
        {
            Dispose(_comApplicationInstances);
            Dispose(_comApplications);
            Dispose(_comAdminCatalog);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destructor of the <see cref="COMAdminCatalogWrapperSingleton"/> class.
        /// </summary>
        ~COMAdminCatalogWrapperSingleton()
        {
            Dispose(false);
        }

        /// <summary>
        /// Release the COM objects
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
