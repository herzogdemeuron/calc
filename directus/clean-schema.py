import json

with open('directus-schema.json') as json_file:
    schema = json.load(json_file)

schema["collections"] = [collection for collection in schema["collections"] if collection["collection"].startswith("calc")]
schema["fields"] = [field for field in schema["fields"] if field["collection"].startswith("calc")]
schema["relations"] = [relation for relation in schema["relations"] if relation["collection"].startswith("calc")]

with open('calc-directus-schema.json', 'w') as outfile:
    json.dump(schema, outfile, indent=4)