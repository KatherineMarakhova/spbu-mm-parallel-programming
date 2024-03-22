import numpy as np

print("This script generates file with random integer values.")
size = int(input("Write the amount of data to be generated: "))
path = input("Path to save: ")

a = np.random.randint(size * 10, size = size)
with open(path, "w") as file:
    file.write(f'{size}\n')
    for i in a:
        file.write(str(i) + ' ')
