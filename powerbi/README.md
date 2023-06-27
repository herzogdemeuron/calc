# PowerBi Directus Query Template

## General

The template uses a python script to make a web request to your Directus instance.
The visuals in the template show you how the tables work together.

## Requirements

- Python 3.10
- Requests
- Pandas ("pip install pandas")
- Matplotlib? not sure about this one.
- A Directus user with read permissions to `lca_calculation_results`
- An API token for that user


## Authentication

You need to adjust the `token` variable in the python query.

Follow these steps:

1. Open the Query Editor by clicking on `Transform Data` in the `Home` tab.
2. In the `APPLIED STEPS` section, click on the cogwheel next to `Source`.
3. Set the `token` variable to your API token using a method of your choice. Examples:
    * Hard Coded
    * From environment variables
    * etc...


## Load Limit

You might need to adjust the number of items being loaded from Directus.
Follow these steps:

1. Open the Query Editor by clicking on `Transform Data` in the `Home` tab.
2. In the `APPLIED STEPS` section, click on the cogwheel next to `Source`.
3. Look for `params={"limit": 300, "sort": "-date_created"}` and change the `limit` to the amount of items you want to load. `-date_created` means that it will start loading from the most recently added item.