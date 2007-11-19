﻿#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.IO;
using Castle.Core.Resource;
using Castle.MicroKernel;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Windsor;
using Castle.Windsor.Configuration;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.Commons.Binsor;

namespace Rhino.Commons
{
    public class RhinoContainer : WindsorContainer
    {
        private bool isDisposed = false;

        public RhinoContainer()
        {
        }

        public RhinoContainer(IWindsorContainer parent)
            : base(parent, new XmlInterpreter(new StaticContentResource("<configuration/>")))
        {
        }

		public RhinoContainer(string fileName)
			: this(fileName, null)
		{
		}

    	public RhinoContainer(string fileName, IEnvironmentInfo env)
			: base(new DefaultKernel(), new BooComponentInstaller(fileName))
        {
			InitalizeFromConfigurationSource((IConfigurationInterpreter)Installer, env);
        }

        public RhinoContainer(IConfigurationInterpreter interpreter)
            : this(interpreter, null)
        {
        }

		public RhinoContainer(IConfigurationInterpreter interpreter, IEnvironmentInfo env)
		{
			InitalizeFromConfigurationSource(interpreter, env);
		}

    	private void InitalizeFromConfigurationSource(IConfigurationInterpreter interpreter,
    	                                              IEnvironmentInfo env)
    	{
            DefaultConversionManager conversionManager = (DefaultConversionManager)Kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey);
            conversionManager.Add(new ConfigurationObjectConverter());

			if (env != null)
			{
				interpreter.EnvironmentName = env.GetEnvironmentName();
			}

            interpreter.ProcessResource(interpreter.Source, Kernel.ConfigurationStore);
            RunInstaller();
        }

        public override T Resolve<T>(string key)
        {
            AssertNotDisposed();
            return base.Resolve<T>(key);
        }

        public override object Resolve(string key)
        {
            AssertNotDisposed();
            return base.Resolve(key);
        }

        public override object Resolve(string key, Type service)
        {
            AssertNotDisposed();
            return base.Resolve(key, service);
        }

        public override object Resolve(Type service)
        {
            AssertNotDisposed();
            return base.Resolve(service);
        }

        private void AssertNotDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException("The container has been disposed!");
        }

        public override void Dispose()
        {
            isDisposed = true;
            base.Dispose();
        }

        public bool IsDisposed
        {
            get { return isDisposed; }
        }
    }
}