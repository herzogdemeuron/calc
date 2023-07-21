import json

with open('directus-schema.json') as json_file:
    schema = json.load(json_file)

schema["collections"] = [collection for collection in schema["collections"] if collection["collection"].startswith("calc")]
schema["fields"] = [field for field in schema["fields"] if field["collection"].startswith("calc")]
schema["relations"] = [relation for relation in schema["relations"] if relation["collection"].startswith("calc")]

# do the same if collection == calc_calculation_results
# schema["collections"] = [collection for collection in schema["collections"] if collection["collection"] != "calc_calculation_results"]
# schema["fields"] = [field for field in schema["fields"] if field["collection"] != "calc_calculation_results"]
# schema["relations"] = [relation for relation in schema["relations"] if relation["collection"] != "calc_calculation_results"]

with open('directus-schema-cleaned.json', 'w') as outfile:
    json.dump(schema, outfile, indent=4)