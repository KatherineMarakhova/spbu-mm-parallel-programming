# MPI Quick sorting by hypercube dissection

### Generate test data first
The python script will prompt you to enter the number of values and the path to save, then generate random values and save them to a file.
```
python generatedata.py
```

### Build the project
To compile the program use **Makefile**
```
make
```

### Execute the code with the number of processes
**Note:** The algorithm runs on **2^N** processors
```
mpiexec -n 4 ./quicksort <path-to-data> output
```
