using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.FluentInterface
{
    internal class SamplePattern
    {
        internal void Create()
        {
            var build = new FluentSample1()
                .Id("id1")
                .Name("name")
                .Version("1.0.0")
                .Build();
        }
        

    }

    /// <summary>
    /// これよりもPattern1のほうがいいです
    /// </summary>
    internal class FluentSample1: IBuilder
    {
        public string _id { get; private set; }
        public string? _name { get; private set; }
        public string? _version { get; private set; }

        public IBuilder Id(string id)
        {
            this._id = id;
            return this;
        }

        public IBuilder Name(string? name)
        {
            this._name = name;
            return this;
        }

        public IBuilder Version(string? version)
        {
            this._version = version;
            return this;
        }

        public IBuilder Build()
        {
            return this;
        }
    }

    interface IBuilder
    {
        IBuilder Id(string id);
        IBuilder Name(string? name);
        IBuilder Version(string? version);
        IBuilder Build();
    }
}
