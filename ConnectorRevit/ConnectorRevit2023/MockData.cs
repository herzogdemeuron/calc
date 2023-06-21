using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Calc.Core;
using Calc.Core.Objects;
using Calc.Core.DirectusAPI;
using System.Configuration;
using System.Security.Policy;
using System.Diagnostics;

namespace Calc.ConnectorRevit
{
    public class MockData
    {
        public static List<Buildup> AllBuildups { get; set; }
        public static void Initiate()
        {
            Component component1 = new Component()
            {
                Material = new Material()
                {
                    Id = 001,
                    Name = "Material 1"
                },
                Amount = 10,
            };
            Component component2 = new Component()
            {
                Material = new Material()
                {
                    Id = 002,
                    Name = "Material 2"
                },
                Amount = 33,
            };
            Component component3 = new Component()
            {
                Material = new Material()
                {
                    Id = 003,
                    Name = "Material 3"
                },
                Amount = 98,
            };

            List<Buildup> buildups = new List<Buildup>
            {
                new Buildup()
                {
                    Id = 001,
                    Name = "Buildup 1",
                    Group = new Group() { Name = "Group a" },
                    Components = new List<Component>() { component1, component2 }
                },

                new Buildup()
                {
                    Id = 002,
                    Name = "Buildup 2",
                    Group = new Group() { Name = "Group b" },
                    Components = new List<Component>() { component3, component1 }
                },

                new Buildup()
                {
                    Id = 003,
                    Name = "Buildup 3",
                    Group = new Group() { Name = "Group c" },
                    Components = new List<Component>() { component2, component3, component1 }
                }
            };


            MockData.AllBuildups = buildups;

            //getting all buildups from directus
            //MockData.AllBuildups = BuildupStorageDriver.GetAllBuildups();
        }

    }
}
