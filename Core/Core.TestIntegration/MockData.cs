using System;
using System.Xml.Linq;
using Calc.Core.Filtering;
using Calc.Core.Objects;
using Newtonsoft.Json.Linq;

namespace Calc.Core.TestIntegration;

public class MockData
{
    public Forest Forest { get; set; } = new Forest() { Name = "Mock Forest" };
    public List<Tree> Trees { get; set; } = new List<Tree>();
    public List<CalcElement> Elements { get; set; } = new List<CalcElement>();
    public List<Buildup> Buildups { get; set; } = new List<Buildup>();
    public Mapping Mapping { get; set; } = new Mapping();


    public MockData()
    {
        MakeForest();
        MakeElements();
        MakeBuildups();
        MakeMapping();
    }
    private void MakeForest()
    {
        
        var tree1 = new Tree
        {
            Name = "Roh_Wand",
            ParentForest = Forest,
            FilterConfig = new GroupCondition
            {
                Operator = "and",
                Conditions = new List<ConditionContainer>
                {
                    new ConditionContainer
                    {
                        Type = "SimpleCondition",
                        Condition = new SimpleCondition
                        {
                            Parameter = "inst:Type",
                            Method = "contains",
                            Value = "WAND"
                        }
                    },
                    new ConditionContainer
                    {
                        Type = "SimpleCondition",
                        Condition = new SimpleCondition
                        {
                            Parameter = "inst:Type",
                            Method = "contains",
                            Value = "ROHB"
                        }
                    }
                }
            },
            BranchConfig = new List<string> { "Grouping", "SubGrouping" }
        };

        var tree2 = new Tree
        {
            Name = "Ausb_Decke",
            ParentForest = Forest,
            FilterConfig = new GroupCondition
            {
                Operator = "and",
                Conditions = new List<ConditionContainer>
                {
                    new ConditionContainer
                    {
                        Type = "SimpleCondition",
                        Condition = new SimpleCondition
                        {
                            Parameter = "inst:Type",
                            Method = "contains",
                            Value = "DECK"
                        }
                    },
                    new ConditionContainer
                    {
                        Type = "SimpleCondition",
                        Condition = new SimpleCondition
                        {
                            Parameter = "inst:Type",
                            Method = "contains",
                            Value = "AUSB"
                        }
                    }
                }
            },
            BranchConfig = new List<string> { "Grouping", "SubGrouping" }
        };

        this.Trees = new List<Tree>
            {
                tree1,
                tree2
            };
        this.Forest.Trees = this.Trees;
        
    }


    private void MakeElements()
    {
        // Mock walls

        string name1 = "elemName1";
        var Fields1 = new Dictionary<string, object>
        {
            { "inst:Type", "WAND_ROHB_300" },
            { "Grouping", "Group1" },
            { "SubGrouping", "SubGroup1" }
        };
        var element1 = new CalcElement("id01",name1, Fields1, length: 14, area: 12, volume: 90);
        this.Elements.Add(element1);

        string name2 = "elemName2";
        var Fields2 = new Dictionary<string, object>
        {
            { "inst:Type", "WAND_ROHB_150" },
            { "Grouping", "Group1" },
            { "SubGrouping", "SubGroup2" }
        };
        var element2 = new CalcElement("id02", name2, Fields2, length: 18, area: 11, volume: 90);
        this.Elements.Add(element2);

        string name3 = "elemName3";
        var Fields3 = new Dictionary<string, object>
        {
            { "inst:Type", "WAND_AUSB_150" },
            { "Grouping", "Group2" },
            { "SubGrouping", "SubGroup2" }
        };
        var element3 = new CalcElement("id03", name3, Fields3, length: 15,area: 13, volume:9);
        this.Elements.Add(element3);

        // Mock floors

        string name4 = "elemName4";
        var Fields4 = new Dictionary<string, object>
        {
            { "inst:Type", "DECK_AUSB_200" },
            { "Grouping", "GroupA" },
            { "SubGrouping", "SubGroupA" }
        };
        var element4 = new CalcElement("id11", name4, Fields4, length: null, area: 33, volume: 190);
        this.Elements.Add(element4);

        string name5 = "elemName5";
        var Fields5 = new Dictionary<string, object>
        {
            { "inst:Type", "DECK_AUSB_200" },
            { "Grouping", "GroupA" },
            { "SubGrouping", "SubGroupA" }
        };
        var element5 = new CalcElement("id12", name5, Fields5, length: null, area: 13, volume: 90);
        this.Elements.Add(element5);

        string name6 = "elemName6";
        var Fields6 = new Dictionary<string, object>
        {
            { "inst:Type", "DECK_AUSB_100" },
            { "Grouping", "GroupA" },
            { "SubGrouping", "SubGroupB" }
        };
        var element6 = new CalcElement("id13", name6, Fields6, length: null, area: null, volume: null);
        this.Elements.Add(element6);
    }

    public void MakeBuildups()
    {
        var group1 = new MaterialGroup
        {
            Name = "MaterialGroup1"
        };

        var material1 = new Material
        {
            Id = 1,
            Name = "Material1",
            KgCO2eA123 = 18,
            Category = "Category1"
        };

        var component1 = new BuildupComponent
        {
            Amount = 0.5m,
            Material = material1
        };

        var material2 = new Material
        {
            Id = 2,
            Name = "Material2",
            KgCO2eA123 = 5,
            Category = "Category2"
        };

        var component2 = new BuildupComponent
        {
            Amount = 0.7m,
            Material = material2
        };

        var material3 = new Material
        {
            Id = 3,
            Name = "Material3",
            KgCO2eA123 = 45,
            Category = "Category1"
        };

        var component3 = new BuildupComponent
        {
            Amount = 0.8m,
            Material = material3
        };


        var buildup1 = new Buildup
        {
            Id = 901,
            Name = "Buildup No.1",
            Unit = Unit.m2
        };

        var buildup2 = new Buildup
        {
            Id = 902,
            Name = "Buildup No.2",
            Unit = Unit.m3
        };


        buildup1.Group = group1;
        buildup1.Components = new List<BuildupComponent>
        {
            component1,
            component2
        };

        buildup2.Group = group1;
        buildup2.Components = new List<BuildupComponent>
        {
            component3
        };


        this.Buildups.Add(buildup1);
        this.Buildups.Add(buildup2);
    }

    public void MakeMapping()
    {
        var project = new Project
        {
            Id = 0,
            ProjectNumber = "123"
        };

        
        this.Mapping = new Mapping
        {
            Name = "Mapping1",
            Project = project,
            MappingItems = new List<MappingItem>
            {
                new MappingItem
                {
                    TreeName = "Roh_Wand",
                    Path = new List<PathItem>(),
                    BuildupIds = new List<int> { 902, 901 }
                },
                new MappingItem
                {
                    TreeName = "Roh_Wand",
                    Path = new List<PathItem>
                    {
                        new PathItem
                        {
                            Parameter = "Grouping",
                            Value = "Group1"
                        },
                        new PathItem
                        {
                            Parameter = "SubGrouping",
                            Value = "SubGroup2"
                        }
                    },
                    BuildupIds = new List<int> { 902 }
                },
                new MappingItem
                {
                    TreeName = "Ausb_Decke",
                    Path = new List<PathItem>
                    {
                        new PathItem
                        {
                            Parameter = "Grouping",
                            Value = "GroupA"
                        },
                        new PathItem
                        {
                            Parameter = "SubGrouping",
                            Value = "SubGroupA"
                        }
                    },
                    BuildupIds = new List<int> { 902 }
                },
                new MappingItem
                {
                    TreeName = "Ausb_Decke",
                    Path = new List<PathItem>
                    {
                        new PathItem
                        {
                            Parameter = "Grouping",
                            Value = "GroupA"
                        },
                        new PathItem
                        {
                            Parameter = "SubGrouping",
                            Value = "SubGroupB"
                        }
                    },
                    BuildupIds = new List<int> { 901 }
                }
            }
        };
    }
}
