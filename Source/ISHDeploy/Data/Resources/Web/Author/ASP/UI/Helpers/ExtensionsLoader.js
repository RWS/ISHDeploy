/// <reference path="../../typings/index.d.ts"/>
// Configuration
var SDL;
(function (SDL) {
    var Client;
    (function (Client) {
        var Configuration;
        (function (Configuration) {
            Configuration.settingsFile = "";
            Configuration.settingsVersion = "";
        })(Configuration = Client.Configuration || (Client.Configuration = {}));
    })(Client = SDL.Client || (SDL.Client = {}));
})(SDL || (SDL = {}));
var Trisoft;
(function (Trisoft) {
    var Helpers;
    (function (Helpers) {
        var ExtensionsLoader;
        (function (ExtensionsLoader) {
            /**
             * Ready callbacks
             */
            ExtensionsLoader.readyCallbacks = Trisoft.Helpers.ExtensionsLoader.readyCallbacks || [];
            /**
             * Indicates if the extensions are loading
             */
            ExtensionsLoader.isLoading = Trisoft.Helpers.ExtensionsLoader.isLoading || false;
            /**
             * When true extension can be used. Otherwise use the addReadyCallback to be notified when ready.
             */
            ExtensionsLoader.extensionsLoaded = Trisoft.Helpers.ExtensionsLoader.extensionsLoaded || false;
            /**
             * Add a callback which is called whenever the extensions are loaded and ready to use.
             *
             * @export
             * @param {(error?: string) => void} callback Callback to be called. Returns an error as first argument if something went wrong.
             */
            function addReadyCallback(callback) {
                ExtensionsLoader.readyCallbacks.push(callback);
            }
            ExtensionsLoader.addReadyCallback = addReadyCallback;
            /**
             * Execute an extension
             *
             * @export
             * @param {string} extensionMethod The method to call.
             * @param {any[]} [params] List of paramters which needed to be provided with the method.
             */
            // tslint:disable-next-line no-any
            function executeExtension(extensionMethod, params) {
                if (Trisoft.Helpers.ExtensionsLoader.extensionsLoaded) {
                    _getExtensionMethod(extensionMethod).apply(this, params);
                }
                else {
                    Trisoft.Helpers.ExtensionsLoader.addReadyCallback(function (error) {
                        if (!error) {
                            _getExtensionMethod(extensionMethod).apply(this, params);
                            return;
                        }
                        console.error("Unable to execute extension with error: " + error);
                    });
                }
            }
            ExtensionsLoader.executeExtension = executeExtension;
            /**
             * Enable the extensions
             *
             * @export
             * @param {string} baseUrl Base url for the extensions directory. Should be pointing to the root directory.
             * @param {string} [extensionsResourceId="Trisoft.Extensions"] Resource id of the extensions.
             */
            function enableExtensions(baseUrl, extensionsResourceId) {
                if (extensionsResourceId === void 0) { extensionsResourceId = "Trisoft.Extensions"; }
                if (!ExtensionsLoader.extensionsLoaded && !ExtensionsLoader.isLoading) {
                    ExtensionsLoader.isLoading = true;
                    _getVersion(baseUrl + "UI/version.txt", function (version) {
                        // Set Configuration
                        SDL.Client.Configuration.settingsFile = baseUrl + "UI/configurationUnhosted.xml" + version;
                        _loadJsFile(baseUrl + "Common/Library/Core/Packages/SDL.Client.Init.js" + version, function () {
                            var loadExtensions = function () {
                                SDL.Client.Resources.ResourceManager.load(extensionsResourceId, function () {
                                    ExtensionsLoader.extensionsLoaded = true;
                                    ExtensionsLoader.isLoading = false;
                                    ExtensionsLoader.readyCallbacks.forEach(function (callback) {
                                        callback();
                                    });
                                }, function (error) {
                                    ExtensionsLoader.extensionsLoaded = true;
                                    ExtensionsLoader.isLoading = false;
                                    console.error("Unable to load extension resources with error: " + error);
                                    ExtensionsLoader.readyCallbacks.forEach(function (callback) {
                                        callback(error);
                                    });
                                });
                            };
                            if (SDL.Client.Application.isInitialized) {
                                loadExtensions();
                            }
                            else {
                                SDL.Client.Application.addReadyCallback(loadExtensions);
                            }
                        });
                    });
                }
            }
            ExtensionsLoader.enableExtensions = enableExtensions;
            function _getExtensionMethod(methodName) {
                var fullName = methodName.split(".");
                var globalWindow = window;
                var extension;
                for (var _i = 0, fullName_1 = fullName; _i < fullName_1.length; _i++) {
                    var method = fullName_1[_i];
                    if (!extension) {
                        extension = globalWindow[method];
                    }
                    else {
                        extension = extension[method];
                    }
                }
                return extension;
            }
            function _getVersion(filename, callback) {
                // the value changes every minute triggering reload of the version file every minute
                // tslint:disable-next-line no-bitwise
                var reload = (new Date().getTime() - 1380000000000) / 60000 | 0;
                _getXhr(filename + "?" + reload, function (result) {
                    if (result) {
                        var m = result.match(/^\s*(confVersion|applicationVersion)\s*=\s*(\d+(\.\d+)*)/m);
                        if (m) {
                            // application configuration version
                            SDL.Client.Configuration.settingsVersion = m[2];
                            callback("?" + SDL.Client.Configuration.settingsVersion);
                        }
                    }
                    else {
                        callback("");
                    }
                }, true);
            }
            function _loadJsFile(filename, callback) {
                _getXhr(filename, function (result) {
                    var globalEval = eval;
                    globalEval(result);
                    callback();
                }, true);
            }
            function _getXhr(url, callback, breakOnError) {
                var xhr;
                if (XMLHttpRequest) {
                    xhr = new XMLHttpRequest();
                }
                if (!xhr) {
                    throw Error("Unable to create XMLHttpRequest object.");
                }
                xhr.onreadystatechange = function () {
                    if (xhr.readyState === 4) {
                        if (breakOnError && xhr.status !== 200) {
                            var error;
                            try {
                                error = xhr.statusText;
                            }
                            catch (err) {
                                error = xhr.responseText || "";
                            }
                            throw Error("Unable to load \"" + url + ": (" + xhr.status + ") " + error);
                        }
                        callback(xhr.responseText || "");
                    }
                };
                xhr.open("GET", url, true);
                xhr.send();
            }
        })(ExtensionsLoader = Helpers.ExtensionsLoader || (Helpers.ExtensionsLoader = {}));
    })(Helpers = Trisoft.Helpers || (Trisoft.Helpers = {}));
})(Trisoft || (Trisoft = {}));

//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIlVJL0hlbHBlcnMvRXh0ZW5zaW9uc0xvYWRlci50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxnREFBZ0Q7QUFFaEQsZ0JBQWdCO0FBQ2hCLElBQU8sR0FBRyxDQUdUO0FBSEQsV0FBTyxHQUFHO0lBQUMsSUFBQSxNQUFNLENBR2hCO0lBSFUsV0FBQSxNQUFNO1FBQUMsSUFBQSxhQUFhLENBRzlCO1FBSGlCLFdBQUEsYUFBYSxFQUFDLENBQUM7WUFDN0IsMEJBQVksR0FBRyxFQUFFLENBQUM7WUFDbEIsNkJBQWUsR0FBRyxFQUFFLENBQUM7UUFDekIsQ0FBQyxFQUhpQixhQUFhLEdBQWIsb0JBQWEsS0FBYixvQkFBYSxRQUc5QjtJQUFELENBQUMsRUFIVSxNQUFNLEdBQU4sVUFBTSxLQUFOLFVBQU0sUUFHaEI7QUFBRCxDQUFDLEVBSE0sR0FBRyxLQUFILEdBQUcsUUFHVDtBQUVELElBQU8sT0FBTyxDQXNLYjtBQXRLRCxXQUFPLE9BQU87SUFBQyxJQUFBLE9BQU8sQ0FzS3JCO0lBdEtjLFdBQUEsT0FBTztRQUFDLElBQUEsZ0JBQWdCLENBc0t0QztRQXRLc0IsV0FBQSxnQkFBZ0IsRUFBQyxDQUFDO1lBWXJDOztlQUVHO1lBQ1EsK0JBQWMsR0FBaUMsT0FBTyxDQUFDLE9BQU8sQ0FBQyxnQkFBZ0IsQ0FBQyxjQUFjLElBQUksRUFBRSxDQUFDO1lBRWhIOztlQUVHO1lBQ1EsMEJBQVMsR0FBWSxPQUFPLENBQUMsT0FBTyxDQUFDLGdCQUFnQixDQUFDLFNBQVMsSUFBSSxLQUFLLENBQUM7WUFFcEY7O2VBRUc7WUFDUSxpQ0FBZ0IsR0FBWSxPQUFPLENBQUMsT0FBTyxDQUFDLGdCQUFnQixDQUFDLGdCQUFnQixJQUFJLEtBQUssQ0FBQztZQUVsRzs7Ozs7ZUFLRztZQUNILDBCQUFpQyxRQUFrQztnQkFDL0QsK0JBQWMsQ0FBQyxJQUFJLENBQUMsUUFBUSxDQUFDLENBQUM7WUFDbEMsQ0FBQztZQUZlLGlDQUFnQixtQkFFL0IsQ0FBQTtZQUVEOzs7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QUFDLE1BQU0sR0FBRyxJQUFJLEdBQUcsS0FBSyxDQUFDLENBQUM7d0JBQy9FLENBQUM7d0JBQ0QsUUFBUSxDQUFDLEdBQUcsQ0FBQyxZQUFZLElBQUksRUFBRSxDQUFDLENBQUM7b0JBQ3JDLENBQUM7Z0JBQ0wsQ0FBQyxDQUFDO2dCQUVGLEdBQUcsQ0FBQyxJQUFJLENBQUMsS0FBSyxFQUFFLEdBQUcsRUFBRSxJQUFJLENBQUMsQ0FBQztnQkFDM0IsR0FBRyxDQUFDLElBQUksRUFBRSxDQUFDO1lBQ2YsQ0FBQztRQUNMLENBQUMsRUF0S3NCLGdCQUFnQixHQUFoQix3QkFBZ0IsS0FBaEIsd0JBQWdCLFFBc0t0QztJQUFELENBQUMsRUF0S2MsT0FBTyxHQUFQLGVBQU8sS0FBUCxlQUFPLFFBc0tyQjtBQUFELENBQUMsRUF0S00sT0FBTyxLQUFQLE9BQU8sUUFzS2IiLCJmaWxlIjoiVUkvSGVscGVycy9FeHRlbnNpb25zTG9hZGVyLmpzIiwic291cmNlc0NvbnRlbnQiOlsiLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uLy4uL3R5cGluZ3MvaW5kZXguZC50c1wiLz5cblxuLy8gQ29uZmlndXJhdGlvblxubW9kdWxlIFNETC5DbGllbnQuQ29uZmlndXJhdGlvbiB7XG4gICAgc2V0dGluZ3NGaWxlID0gXCJcIjtcbiAgICBzZXR0aW5nc1ZlcnNpb24gPSBcIlwiO1xufVxuXG5tb2R1bGUgVHJpc29mdC5IZWxwZXJzLkV4dGVuc2lvbnNMb2FkZXIge1xuXG4gICAgaW50ZXJmYWNlIElXaW5kb3cgZXh0ZW5kcyBXaW5kb3cge1xyXG4gICAgICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZSBuby1hbnlcclxuICAgICAgICBbaW5kZXg6IHN0cmluZ106IGFueTtcclxuICAgIH1cblxuICAgIGludGVyZmFjZSBJRnVuY3Rpb24gZXh0ZW5kcyBGdW5jdGlvbiB7XHJcbiAgICAgICAgLy8gdHNsaW50OmRpc2FibGUtbmV4dC1saW5lIG5vLWFueVxyXG4gICAgICAgIFtpbmRleDogc3RyaW5nXTogYW55O1xyXG4gICAgfVxuXG4gICAgLyoqXHJcbiAgICAgKiBSZWFkeSBjYWxsYmFja3NcclxuICAgICAqL1xuICAgIGV4cG9ydCB2YXIgcmVhZHlDYWxsYmFja3M6IHsgKGVycm9yPzogc3RyaW5nKTogdm9pZCB9W10gPSBUcmlzb2Z0LkhlbHBlcnMuRXh0ZW5zaW9uc0xvYWRlci5yZWFkeUNhbGxiYWNrcyB8fCBbXTtcblxuICAgIC8qKlxyXG4gICAgICogSW5kaWNhdGVzIGlmIHRoZSBleHRlbnNpb25zIGFyZSBsb2FkaW5nXHJcbiAgICAgKi9cbiAgICBleHBvcnQgdmFyIGlzTG9hZGluZzogYm9vbGVhbiA9IFRyaXNvZnQuSGVscGVycy5FeHRlbnNpb25zTG9hZGVyLmlzTG9hZGluZyB8fCBmYWxzZTtcblxuICAgIC8qKlxuICAgICAqIFdoZW4gdHJ1ZSBleHRlbnNpb24gY2FuIGJlIHVzZWQuIE90aGVyd2lzZSB1c2UgdGhlIGFkZFJlYWR5Q2FsbGJhY2sgdG8gYmUgbm90aWZpZWQgd2hlbiByZWFkeS5cbiAgICAgKi9cbiAgICBleHBvcnQgdmFyIGV4dGVuc2lvbnNMb2FkZWQ6IGJvb2xlYW4gPSBUcmlzb2Z0LkhlbHBlcnMuRXh0ZW5zaW9uc0xvYWRlci5leHRlbnNpb25zTG9hZGVkIHx8IGZhbHNlO1xuXG4gICAgLyoqXG4gICAgICogQWRkIGEgY2FsbGJhY2sgd2hpY2ggaXMgY2FsbGVkIHdoZW5ldmVyIHRoZSBleHRlbnNpb25zIGFyZSBsb2FkZWQgYW5kIHJlYWR5IHRvIHVzZS5cbiAgICAgKlxuICAgICAqIEBleHBvcnRcbiAgICAgKiBAcGFyYW0geyhlcnJvcj86IHN0cmluZykgPT4gdm9pZH0gY2FsbGJhY2sgQ2FsbGJhY2sgdG8gYmUgY2FsbGVkLiBSZXR1cm5zIGFuIGVycm9yIGFzIGZpcnN0IGFyZ3VtZW50IGlmIHNvbWV0aGluZyB3ZW50IHdyb25nLlxuICAgICAqL1xuICAgIGV4cG9ydCBmdW5jdGlvbiBhZGRSZWFkeUNhbGxiYWNrKGNhbGxiYWNrOiAoZXJyb3I/OiBzdHJpbmcpID0+IHZvaWQpOiB2b2lkIHtcbiAgICAgICAgcmVhZHlDYWxsYmFja3MucHVzaChjYWxsYmFjayk7XG4gICAgfVxuXG4gICAgLyoqXG4gICAgICogRXhlY3V0ZSBhbiBleHRlbnNpb25cbiAgICAgKlxuICAgICAqIEBleHBvcnRcbiAgICAgKiBAcGFyYW0ge3N0cmluZ30gZXh0ZW5zaW9uTWV0aG9kIFRoZSBtZXRob2QgdG8gY2FsbC5cbiAgICAgKiBAcGFyYW0ge2FueVtdfSBbcGFyYW1zXSBMaXN0IG9mIHBhcmFtdGVycyB3aGljaCBuZWVkZWQgdG8gYmUgcHJvdmlkZWQgd2l0aCB0aGUgbWV0aG9kLlxuICAgICAqL1xuICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZSBuby1hbnlcbiAgICBleHBvcnQgZnVuY3Rpb24gZXhlY3V0ZUV4dGVuc2lvbihleHRlbnNpb25NZXRob2Q6IHN0cmluZywgcGFyYW1zPzogYW55W10pOiB2b2lkIHtcbiAgICAgICAgaWYgKFRyaXNvZnQuSGVscGVycy5FeHRlbnNpb25zTG9hZGVyLmV4dGVuc2lvbnNMb2FkZWQpIHtcbiAgICAgICAgICAgIF9nZXRFeHRlbnNpb25NZXRob2QoZXh0ZW5zaW9uTWV0aG9kKS5hcHBseSh0aGlzLCBwYXJhbXMpO1xuICAgICAgICB9IGVsc2Uge1xuICAgICAgICAgICAgVHJpc29mdC5IZWxwZXJzLkV4dGVuc2lvbnNMb2FkZXIuYWRkUmVhZHlDYWxsYmFjayhmdW5jdGlvbiAoZXJyb3I6IHN0cmluZyk6IHZvaWQge1xuICAgICAgICAgICAgICAgIGlmICghZXJyb3IpIHtcbiAgICAgICAgICAgICAgICAgICAgX2dldEV4dGVuc2lvbk1ldGhvZChleHRlbnNpb25NZXRob2QpLmFwcGx5KHRoaXMsIHBhcmFtcyk7XG4gICAgICAgICAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgY29uc29sZS5lcnJvcihgVW5hYmxlIHRvIGV4ZWN1dGUgZXh0ZW5zaW9uIHdpdGggZXJyb3I6ICR7ZXJyb3J9YCk7XG4gICAgICAgICAgICB9KTtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIC8qKlxuICAgICAqIEVuYWJsZSB0aGUgZXh0ZW5zaW9uc1xuICAgICAqXG4gICAgICogQGV4cG9ydFxuICAgICAqIEBwYXJhbSB7c3RyaW5nfSBiYXNlVXJsIEJhc2UgdXJsIGZvciB0aGUgZXh0ZW5zaW9ucyBkaXJlY3RvcnkuIFNob3VsZCBiZSBwb2ludGluZyB0byB0aGUgcm9vdCBkaXJlY3RvcnkuXG4gICAgICogQHBhcmFtIHtzdHJpbmd9IFtleHRlbnNpb25zUmVzb3VyY2VJZD1cIlRyaXNvZnQuRXh0ZW5zaW9uc1wiXSBSZXNvdXJjZSBpZCBvZiB0aGUgZXh0ZW5zaW9ucy5cbiAgICAgKi9cbiAgICBleHBvcnQgZnVuY3Rpb24gZW5hYmxlRXh0ZW5zaW9ucyhiYXNlVXJsOiBzdHJpbmcsIGV4dGVuc2lvbnNSZXNvdXJjZUlkOiBzdHJpbmcgPSBcIlRyaXNvZnQuRXh0ZW5zaW9uc1wiKTogdm9pZCB7XG4gICAgICAgIGlmICghZXh0ZW5zaW9uc0xvYWRlZCAmJiAhaXNMb2FkaW5nKSB7XG4gICAgICAgICAgICBpc0xvYWRpbmcgPSB0cnVlO1xuICAgICAgICAgICAgX2dldFZlcnNpb24oYmFzZVVybCArIFwiVUkvdmVyc2lvbi50eHRcIiwgKHZlcnNpb24pID0+IHtcbiAgICAgICAgICAgICAgICAvLyBTZXQgQ29uZmlndXJhdGlvblxuICAgICAgICAgICAgICAgIFNETC5DbGllbnQuQ29uZmlndXJhdGlvbi5zZXR0aW5nc0ZpbGUgPSBiYXNlVXJsICsgXCJVSS9jb25maWd1cmF0aW9uVW5ob3N0ZWQueG1sXCIgKyB2ZXJzaW9uO1xuICAgICAgICAgICAgICAgIF9sb2FkSnNGaWxlKGJhc2VVcmwgKyBcIkNvbW1vbi9MaWJyYXJ5L0NvcmUvUGFja2FnZXMvU0RMLkNsaWVudC5Jbml0LmpzXCIgKyB2ZXJzaW9uLCAoKSA9PiB7XG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IGxvYWRFeHRlbnNpb25zID0gKCk6IHZvaWQgPT4ge1xuICAgICAgICAgICAgICAgICAgICAgICAgU0RMLkNsaWVudC5SZXNvdXJjZXMuUmVzb3VyY2VNYW5hZ2VyLmxvYWQoZXh0ZW5zaW9uc1Jlc291cmNlSWQsICgpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBleHRlbnNpb25zTG9hZGVkID0gdHJ1ZTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpc0xvYWRpbmcgPSBmYWxzZTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICByZWFkeUNhbGxiYWNrcy5mb3JFYWNoKGNhbGxiYWNrID0+IHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgY2FsbGJhY2soKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9KTtcbiAgICAgICAgICAgICAgICAgICAgICAgIH0sIGVycm9yID0+IHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBleHRlbnNpb25zTG9hZGVkID0gdHJ1ZTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpc0xvYWRpbmcgPSBmYWxzZTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjb25zb2xlLmVycm9yKGBVbmFibGUgdG8gbG9hZCBleHRlbnNpb24gcmVzb3VyY2VzIHdpdGggZXJyb3I6ICR7ZXJyb3J9YCk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcmVhZHlDYWxsYmFja3MuZm9yRWFjaChjYWxsYmFjayA9PiB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNhbGxiYWNrKGVycm9yKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9KTtcbiAgICAgICAgICAgICAgICAgICAgICAgIH0pO1xuICAgICAgICAgICAgICAgICAgICB9O1xuICAgICAgICAgICAgICAgICAgICBpZiAoU0RMLkNsaWVudC5BcHBsaWNhdGlvbi5pc0luaXRpYWxpemVkKSB7XG4gICAgICAgICAgICAgICAgICAgICAgICBsb2FkRXh0ZW5zaW9ucygpO1xuICAgICAgICAgICAgICAgICAgICB9IGVsc2Uge1xuICAgICAgICAgICAgICAgICAgICAgICAgU0RMLkNsaWVudC5BcHBsaWNhdGlvbi5hZGRSZWFkeUNhbGxiYWNrKGxvYWRFeHRlbnNpb25zKTtcbiAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIH0pO1xuICAgICAgICAgICAgfSk7XG4gICAgICAgIH1cbiAgICB9XG5cbiAgICBmdW5jdGlvbiBfZ2V0RXh0ZW5zaW9uTWV0aG9kKG1ldGhvZE5hbWU6IHN0cmluZyk6IEZ1bmN0aW9uIHtcbiAgICAgICAgbGV0IGZ1bGxOYW1lID0gbWV0aG9kTmFtZS5zcGxpdChcIi5cIik7XG4gICAgICAgIGxldCBnbG9iYWxXaW5kb3c6IElXaW5kb3cgPSB3aW5kb3c7XG4gICAgICAgIGxldCBleHRlbnNpb246IElGdW5jdGlvbjtcbiAgICAgICAgZm9yIChsZXQgbWV0aG9kIG9mIGZ1bGxOYW1lKSB7XG4gICAgICAgICAgICBpZiAoIWV4dGVuc2lvbikge1xuICAgICAgICAgICAgICAgIGV4dGVuc2lvbiA9IGdsb2JhbFdpbmRvd1ttZXRob2RdIGFzIElGdW5jdGlvbjtcbiAgICAgICAgICAgIH0gZWxzZSB7XG4gICAgICAgICAgICAgICAgZXh0ZW5zaW9uID0gZXh0ZW5zaW9uW21ldGhvZF0gYXMgSUZ1bmN0aW9uO1xuICAgICAgICAgICAgfVxuICAgICAgICB9XG4gICAgICAgIHJldHVybiBleHRlbnNpb247XG4gICAgfVxuXG4gICAgZnVuY3Rpb24gX2dldFZlcnNpb24oZmlsZW5hbWU6IHN0cmluZywgY2FsbGJhY2s6ICh2ZXJzaW9uOiBzdHJpbmcpID0+IHZvaWQpOiB2b2lkIHtcbiAgICAgICAgLy8gdGhlIHZhbHVlIGNoYW5nZXMgZXZlcnkgbWludXRlIHRyaWdnZXJpbmcgcmVsb2FkIG9mIHRoZSB2ZXJzaW9uIGZpbGUgZXZlcnkgbWludXRlXG4gICAgICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZSBuby1iaXR3aXNlXG4gICAgICAgIHZhciByZWxvYWQgPSAobmV3IERhdGUoKS5nZXRUaW1lKCkgLSAxMzgwMDAwMDAwMDAwKSAvIDYwMDAwIHwgMDtcbiAgICAgICAgX2dldFhocihmaWxlbmFtZSArIFwiP1wiICsgcmVsb2FkLCBmdW5jdGlvbiAocmVzdWx0OiBzdHJpbmcpOiB2b2lkIHtcbiAgICAgICAgICAgIGlmIChyZXN1bHQpIHtcbiAgICAgICAgICAgICAgICB2YXIgbTogUmVnRXhwTWF0Y2hBcnJheSA9IHJlc3VsdC5tYXRjaCgvXlxccyooY29uZlZlcnNpb258YXBwbGljYXRpb25WZXJzaW9uKVxccyo9XFxzKihcXGQrKFxcLlxcZCspKikvbSk7XG4gICAgICAgICAgICAgICAgaWYgKG0pIHtcbiAgICAgICAgICAgICAgICAgICAgLy8gYXBwbGljYXRpb24gY29uZmlndXJhdGlvbiB2ZXJzaW9uXG4gICAgICAgICAgICAgICAgICAgIFNETC5DbGllbnQuQ29uZmlndXJhdGlvbi5zZXR0aW5nc1ZlcnNpb24gPSBtWzJdO1xuICAgICAgICAgICAgICAgICAgICBjYWxsYmFjayhcIj9cIiArIFNETC5DbGllbnQuQ29uZmlndXJhdGlvbi5zZXR0aW5nc1ZlcnNpb24pO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIH0gZWxzZSB7XG4gICAgICAgICAgICAgICAgY2FsbGJhY2soXCJcIik7XG4gICAgICAgICAgICB9XG4gICAgICAgIH0sIHRydWUpO1xuICAgIH1cblxuICAgIGZ1bmN0aW9uIF9sb2FkSnNGaWxlKGZpbGVuYW1lOiBzdHJpbmcsIGNhbGxiYWNrOiAoKSA9PiB2b2lkKTogdm9pZCB7XG4gICAgICAgIF9nZXRYaHIoZmlsZW5hbWUsIGZ1bmN0aW9uIChyZXN1bHQ6IHN0cmluZyk6IHZvaWQge1xuICAgICAgICAgICAgdmFyIGdsb2JhbEV2YWwgPSBldmFsO1xuICAgICAgICAgICAgZ2xvYmFsRXZhbChyZXN1bHQpO1xuICAgICAgICAgICAgY2FsbGJhY2soKTtcbiAgICAgICAgfSwgdHJ1ZSk7XG4gICAgfVxuXG4gICAgZnVuY3Rpb24gX2dldFhocih1cmw6IHN0cmluZywgY2FsbGJhY2s6IChyZXN1bHQ6IHN0cmluZykgPT4gdm9pZCwgYnJlYWtPbkVycm9yOiBib29sZWFuKTogdm9pZCB7XG4gICAgICAgIHZhciB4aHI6IFhNTEh0dHBSZXF1ZXN0O1xuICAgICAgICBpZiAoWE1MSHR0cFJlcXVlc3QpIHtcbiAgICAgICAgICAgIHhociA9IG5ldyBYTUxIdHRwUmVxdWVzdCgpO1xuICAgICAgICB9XG5cbiAgICAgICAgaWYgKCF4aHIpIHtcbiAgICAgICAgICAgIHRocm93IEVycm9yKFwiVW5hYmxlIHRvIGNyZWF0ZSBYTUxIdHRwUmVxdWVzdCBvYmplY3QuXCIpO1xuICAgICAgICB9XG4gICAgICAgIHhoci5vbnJlYWR5c3RhdGVjaGFuZ2UgPSBmdW5jdGlvbiAoKTogdm9pZCB7XG4gICAgICAgICAgICBpZiAoeGhyLnJlYWR5U3RhdGUgPT09IDQpIHsgLy8gNCBpcyAncmVhZHknXG4gICAgICAgICAgICAgICAgaWYgKGJyZWFrT25FcnJvciAmJiB4aHIuc3RhdHVzICE9PSAyMDApIHtcbiAgICAgICAgICAgICAgICAgICAgdmFyIGVycm9yOiBzdHJpbmc7XG4gICAgICAgICAgICAgICAgICAgIHRyeSB7XG4gICAgICAgICAgICAgICAgICAgICAgICBlcnJvciA9IHhoci5zdGF0dXNUZXh0O1xuICAgICAgICAgICAgICAgICAgICB9IGNhdGNoIChlcnIpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgIGVycm9yID0geGhyLnJlc3BvbnNlVGV4dCB8fCBcIlwiO1xuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgIHRocm93IEVycm9yKFwiVW5hYmxlIHRvIGxvYWQgXFxcIlwiICsgdXJsICsgXCI6IChcIiArIHhoci5zdGF0dXMgKyBcIikgXCIgKyBlcnJvcik7XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIGNhbGxiYWNrKHhoci5yZXNwb25zZVRleHQgfHwgXCJcIik7XG4gICAgICAgICAgICB9XG4gICAgICAgIH07XG5cbiAgICAgICAgeGhyLm9wZW4oXCJHRVRcIiwgdXJsLCB0cnVlKTtcbiAgICAgICAgeGhyLnNlbmQoKTtcbiAgICB9XG59XG4iXSwic291cmNlUm9vdCI6Ii4uLy4uLyJ9
