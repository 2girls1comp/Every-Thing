# Create two lists: one for the strings that include your substring (category),
# the other for the ones that don't (nonCategory)
category = []
nonCategory = []

propListFile = "PropList.txt"
keyword = "leaves"

# Open your source file an read every line into a list "lines"
with open(propListFile, "r") as file:
    lines = file.readlines()

# Divide the lines into the according lists
for line in lines:
    if keyword in line:
        category.append(line)
    else:
        nonCategory.append(line)

# Create or overwrite a file with the category list
with open(keyword+".txt", "w") as file:
    for x in category:
        file.write(x)

# Overwrite the source file withe nonCategory list
with open(propListFile, "w") as file:
    for x in nonCategory:
        file.write(x)
