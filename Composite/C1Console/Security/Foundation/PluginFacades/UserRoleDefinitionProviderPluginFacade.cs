﻿using System;
using System.Collections.Generic;
using System.Configuration;
using Composite.Core.Collections.Generic;
using Composite.Core.Configuration;
using Composite.C1Console.Events;
using Composite.C1Console.Security.BuildinPlugins.BuildinUserPermissionDefinitionProvider;
using Composite.C1Console.Security.Plugins.UserPermissionDefinitionProvider;
using Composite.C1Console.Security.Plugins.UserPermissionDefinitionProvider.Runtime;


namespace Composite.C1Console.Security.Foundation.PluginFacades
{
    internal static class UserPermissionDefinitionProviderPluginFacade
    {
        private static ResourceLocker<Resources> _resourceLocker = new ResourceLocker<Resources>(new Resources(), Resources.DoInitializeResources);



        static UserPermissionDefinitionProviderPluginFacade()
        {
            GlobalEventSystemFacade.SubscribeToFlushEvent(OnFlushEvent);
        }



        public static bool HasConfiguration()
        {
            return (ConfigurationServices.ConfigurationSource != null) &&
                   (ConfigurationServices.ConfigurationSource.GetSection(UserPermissionDefinitionProviderSettings.SectionName) != null);
        }



        public static IEnumerable<UserPermissionDefinition> AllUserPermissionDefinitions
        {
            get
            {
                return _resourceLocker.Resources.Plugin.AllUserPermissionDefinitions;
            }
        }



        public static bool CanAlterDefinitions
        {
            get
            {
                return _resourceLocker.Resources.Plugin.CanAlterDefinitions;
            }
        }



        public static void SetUserPermissionDefinition(UserPermissionDefinition userPermissionDefinition)
        {
            if (userPermissionDefinition == null) throw new ArgumentNullException("userPermissionDefinition");
            if (string.IsNullOrEmpty(userPermissionDefinition.SerializedEntityToken)) throw new ArgumentNullException("userPermissionDefinition");
            if (string.IsNullOrEmpty(userPermissionDefinition.Username)) throw new ArgumentNullException("userPermissionDefinition");

            _resourceLocker.Resources.Plugin.SetUserPermissionDefinition(userPermissionDefinition);
        }



        public static void RemoveUserPermissionDefinition(UserToken userToken, string serializedEntityToken)
        {
            if (userToken == null) throw new ArgumentNullException("userToken");
            if (string.IsNullOrEmpty(serializedEntityToken)) throw new ArgumentNullException("serializedEntityToken");

            _resourceLocker.Resources.Plugin.RemoveUserPermissionDefinition(userToken, serializedEntityToken);
        }



        public static IEnumerable<UserPermissionDefinition> GetPermissionsByUser(string userName)
        {
            return _resourceLocker.Resources.Plugin.GetPermissionsByUser(userName);
        }



        private static void Flush()
        {
            _resourceLocker.ResetInitialization();
        }



        private static void OnFlushEvent(FlushEventArgs args)
        {
            Flush();
        }



        private static void HandleConfigurationError(Exception ex)
        {
            Flush();

            throw new ConfigurationErrorsException(string.Format("Failed to load the configuration section '{0}' from the configuration.", UserPermissionDefinitionProviderSettings.SectionName), ex);
        }



        private sealed class Resources
        {
            public UserPermissionDefinitionProviderFactory Factory { get; set; }
            public IUserPermissionDefinitionProvider Plugin { get; set; }

            public static void DoInitializeResources(Resources resources)
            {
                if (HasConfiguration())
                {
                    try
                    {
                        resources.Factory = new UserPermissionDefinitionProviderFactory();
                        resources.Plugin = resources.Factory.CreateDefault();
                    }
                    catch (ArgumentException ex)
                    {
                        HandleConfigurationError(ex);
                    }
                    catch (ConfigurationErrorsException ex)
                    {
                        HandleConfigurationError(ex);
                    }
                }
                else
                {
                    resources.Plugin = new BuildinUserPermissionDefinitionProvider();
                }
            }
        }
    }
}
