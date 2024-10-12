<div align="center">
   <picture>
      <source media="(prefers-color-scheme: dark)" srcset="./images/icon_calc_light.png" width="180" height="180">
      <img alt="calc logo" src="./images/icon_calc_dark.png" width="150" height="150">
   </picture>
</div>
<br clear="left">

## Calc Overview
Calc is a tool for architects to rapidly assess the environmental impact of early design phases. Our open data approach leverages:
- **Autodesk Revit** for design authoring
- **Directus** for centralized database management
- **Speckle** for geometry snapshot handling and 3D dashboard visuals

This repository houses the core applications we've developed in-house to facilitate this workflow.

![overview](./images/overview.png)


## Key Features
👪 Role-based data management with Directus UI

🧰 Custom assemblies using material data and Revit model groups

🧱 Efficient Revit project breakdown via query templates

🥅 Seamless assembly-to-model assignment

🤩 Real-time Revit visualization for model branching and assembly validation

👚 Automated calculations uploaded to Directus using Calc schema

## Workflow Steps

### 1. Configure Data in Directus
Set up material library and query templates in the Directus database.

<img src="./images/database.png" width="1100"/>

### 2. Design Assemblies with Calc Builder
Use **Calc Builder** to create and manage custom assemblies.

<img src="./images/demo_calc_builder.gif" width="1100" />

### 3. Apply and Calculate with Calc Project
Use **Calc Project** to assign assemblies to project elements and generate calculation snapshots.

<img src="./images/demo_calc_project.gif" width="1100"/>

## Get Started

👉 [Quick Start Guide](./quick_start)

## Calc Schema

The Calc Schema is designed to capture calculation results as snapshots, facilitating data exchange within the Calc workflow.

### Assembly Snapshot

Represents calculation results for a single predefined assembly:

```json
{ 
  "query_name":null, 
  "assembly_code":"DECK_SYST_STBE_DE11.001",
  "assembly_group":"reasonable group",		
  "assembly_name":"pretty assembly",
  "assembly_unit":"m2",
  "element_types":
    [
      {
        "element_type_id":"789",
        "element_ids":null,
        "element_amount":null,
        "materials":
          [
            {
              "material_function": "Basement Retaining Walls",
              "material_source_uuid": "06.003",
              "material_source": "KBOB (2023) - German",
              "material_name":"cool material",
              "material_unit":"m3",
              "material_amount":0.5,
              "material_carbon_a1a3": 6044.5,
              "material_grey_energy_fabrication_total": 26611.5,
              "calculated_carbon_a1a3": 524.8,
              "calculated_grey_energy_fabrication_total": 2310.48
            }
          ]
      }
    ]
}
```

### Project Snapshot

The Project Snapshot bundles multiple Assembly Snapshots with metadata:

```json
{
  "project_number":"123",
  "project_name":"snazzy project",
  "query_template":"smart template",
  "location":"GER",
  "lca_method":"somehow",
  "area":432.1,
  "life_span":50,
  "stages":["A1A3"],
  "impact_categories":["GWP","GE"]
  "assemblies":
    [
      {
        "query_name":"big tree",
        "assembly_code": "DECK_SYST_STBE_DE11.001",
        "assembly_group":"reasonable group",		
        "assembly_name":"pretty assembly",					
        "assembly_unit": "m2",
        "element_types":
          [
            {
              "element_type_id":null,
              "element_ids":[1,2,3],
              "element_amount": 13.566,
              "materials":
                [
                  {
                    "material_function": "Basement Retaining Walls",
                    "material_source_uuid": "06.003",
                    "material_source": "KBOB (2023) - German",
                    "material_name":"cool material",
                    "material_unit":"m3",
                    "material_amount":0.5,
                    "material_carbon_a1a3": 6044.5,
                    "material_grey_energy_fabrication_total": 26611.5,
                    "calculated_carbon_a1a3": 524.8,
                    "calculated_grey_energy_fabrication_total": 2310.48
                  }
                ]
            }
          ]		  
      }
    ]
}
```
### Notice:
- Assemblies are grouped together when they have identical values for all three fields: `query_name`, `assembly_code`, and `assembly_group`.
- Within each assembly group, element types are further grouped by `element_type_id`.
- Materials within each element type are grouped when they have matching values for all three fields: `material_function`, `material_source_uuid`, and `material_source`.

