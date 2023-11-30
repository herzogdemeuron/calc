using System;
using System.Xml.Linq;
using Calc.Core.Filtering;
using Calc.Core.Objects;
using Newtonsoft.Json.Linq;

namespace Calc.Core.TestIntegration;

public class MockData
{
    public List<Tree> Trees = new();
    public List<CalcElement> Elements = new();
    public Forest Forest = new() { Name = "MockForest" };
    public decimal Area = 100m;
    public decimal gwp123 = 123m;
    public decimal Amount = 0.33m;

    public MockData()
    {
        CreateTrees();
        CreateElements();
    }
    private void CreateTrees()
    {
        var tree1 = new Tree
        {
            Name = "Roh Wand",
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
            Name = "Ausb Decke",
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

        this.Trees.Add(tree1);
        this.Trees.Add(tree2);

        this.Forest.Trees = this.Trees;
    }


    private void CreateElements()
    {
        // Mock walls

        string name1 = "elemName1";
        var Fields1 = new Dictionary<string, object>
        {
            { "inst:Type", "WAND_ROHB_300" },
            { "Grouping", "Group1" },
            { "SubGrouping", "SubGroup1" }
        };
        var element1 = new CalcElement("id01",name1, Fields1, length: null, area: null, volume: null);
        this.Elements.Add(element1);

        string name2 = "elemName2";
        var Fields2 = new Dictionary<string, object>
        {
            { "inst:Type", "WAND_ROHB_150" },
            { "Grouping", "Group1" },
            { "SubGrouping", "SubGroup2" }
        };
        var element2 = new CalcElement("id02", name1, Fields2, length: null, area: null, volume: null);
        this.Elements.Add(element2);

        string name3 = "elemName3";
        var Fields3 = new Dictionary<string, object>
        {
            { "inst:Type", "WAND_AUSB_150" },
            { "Grouping", "Group2" },
            { "SubGrouping", "SubGroup2" }
        };
        var element3 = new CalcElement("id03", name3, Fields3, length: null,area: 0, volume:null);
        this.Elements.Add(element3);

        // Mock floors
        string name4 = "elemName4";
        var Fields4 = new Dictionary<string, object>
        {
            { "inst:Type", "DECK_AUSB_200" },
            { "Grouping", "GroupA" },
            { "SubGrouping", "SubGroupA" }
        };
        var element4 = new CalcElement("id11", name4, Fields4, length: null, area: 0, volume: null);
        this.Elements.Add(element4);

        string name5 = "elemName5";
        var Fields5 = new Dictionary<string, object>
        {
            { "inst:Type", "DECK_AUSB_200" },
            { "Grouping", "GroupA" },
            { "SubGrouping", "SubGroupA" }
        };
        var element5 = new CalcElement("id12", name5, Fields5, length: null, area: null, volume: null);
        this.Elements.Add(element5);

        string name6 = "elemName6";
        var Fields6 = new Dictionary<string, object>
        {
            { "inst:Type", "DECK_AUSB_100" },
            { "Grouping", "GroupA" },
            { "SubGrouping", "SubGroupB" }
        };
        var element6 = new CalcElement("id13", name6, Fields6, length: null, area: 70, volume: null);
        this.Elements.Add(element6);
    }

    public void AssignBuildups(Tree tree)
    {
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

        var buildup1 = new Buildup
        {
            Id = 9,
            Name = "Flat Concrete Slab",
            Unit = Unit.m2
        };

        var group1 = new MaterialGroup
        {
            Name = "MaterialGroup1"
        };

        buildup1.Group = group1;
        buildup1.Components = new List<BuildupComponent>
        {
            component1
        };
        tree.SubBranches[0].Buildup = buildup1;
    }
}
