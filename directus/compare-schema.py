import json

# print the difference between two json files

with open('directus-schema-cleaned.json') as json_file:
    schemaNew = json.load(json_file)

with open('calc-directus-schema.json') as json_file:
    schemaOld = json.load(json_file)

print("Collections:")
for collectionNew in schemaNew["collections"]:
    found = False
    for collectionOld in schemaOld["collections"]:
        if collectionNew["collection"] == collectionOld["collection"]:
            found = True
            break
    if not found:
        print("New collection: {}".format(collectionNew["collection"]))
print("")
print("Fields:")
for fieldNew in schemaNew["fields"]:
    found = False
    for fieldOld in schemaOld["fields"]:
        if fieldNew["field"] == fieldOld["field"]:
            found = True
            break
    if not found:
        print("New field: {} | in collection: {}".format(fieldNew["field"], fieldNew["collection"]))
print("")
print("Relations:")
for relationNew in schemaNew["relations"]:
    found = False
    for relationOld in schemaOld["relations"]:
        if relationNew["field"] == relationOld["field"] and relationNew["related_collection"] == relationOld["related_collection"]:
            found = True
            break
    if not found:
        print("New relation: {}".format(relationNew["relation"]))
print("")
print("Collections:")
for collectionOld in schemaOld["collections"]:
    found = False
    for collectionNew in schemaNew["collections"]:
        if collectionOld["collection"] == collectionNew["collection"]:
            found = True
            break
    if not found:
        print("Deleted collection: {}".format(collectionOld["collection"]))
print("")
print("Fields:")
for fieldOld in schemaOld["fields"]:
    found = False
    for fieldNew in schemaNew["fields"]:
        if fieldOld["field"] == fieldNew["field"]:
            found = True
            break
    if not found:
        print("Deleted field: {}".format(fieldOld["field"]))
print("")
print("Relations:")
for relationOld in schemaOld["relations"]:
    found = False
    for relationNew in schemaNew["relations"]:
        if relationOld["field"] == relationNew["field"] and relationOld["related_collection"] == relationNew["related_collection"]:
            found = True
            break
    if not found:
        print("Deleted relation: {}".format(relationOld["relation"]))
