Back to [Calc](https://github.com/herzogdemeuron/calc#readme)

# PowerBi Directus Query Template

The template uses a python scripts to make a web requests to your Directus instance.
The visuals in the template show you how the tables work together.
You need to make a few adjustments to the template to make it work for you.

## Requirements

- Python 3.10
- Requests
- Pandas ("pip install pandas")
- Matplotlib? not sure about this one.
- A Directus user with read permissions to `calc_calculation_results`
- An API token for that user

## Authentication

Follow these steps:

1. You need to adjust the `token` and `url` variables in the two attached python files and copy the code into the respective queryies in PowerBi.
    * [powerbi_results_query](powerbi_results_query.py)
    * [powerbi_projects_query](powerbi_projects_query.py)
2. Open the Query Editor by clicking on `Transform Data` in the `Home` tab.
3. Select the `calc_calculation_results` query on the left.
4. In the `APPLIED STEPS` section on the right, click on the cogwheel next to `Source`.
5. Copy the code from the [powerbi_results_query](powerbi_results_query.py) that you just edited.
6. Repeat steps 3-5 for the [powerbi_projects_query](powerbi_projects_query.py) on the `calc_architecture_projects` query.

## Load Limit

You might need to adjust the number of items being loaded from Directus.
Complete the steps outlines in [Authentication](#authentication) first, then follow these steps:

1. Open the Query Editor by clicking on `Transform Data` in the `Home` tab.
2. In the `APPLIED STEPS` section, click on the cogwheel next to `Source`.
3. Select the `calc_calculation_results` query on the left.
4. Look for `params={"limit": 300, "sort": "-date_created"}` and change the `limit` to the amount of items you want to load. `-date_created` means that it will start loading from the most recently added item.