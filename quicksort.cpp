#include <algorithm>
#include <fstream>
#include <assert.h>
#include <iostream>
#include <iterator>
#include <mpi.h>
#include <vector>

void quicksort(std::vector<int> &arr, int start, int end)
{
    if (start >= end)
        return;

    auto partitionPoint =
        std::partition(
            arr.begin() + start + 1, arr.begin() + end + 1,
            [pivot = arr[start]](int elem) { return elem < pivot; }) -
        arr.begin();

    std::swap(arr[start], arr[partitionPoint - 1]);
    quicksort(arr, start, partitionPoint - 2);
    quicksort(arr, partitionPoint, end);
}

std::vector<int> merge(const std::vector<int> &arr1,
                       const std::vector<int> &arr2)
{
    std::vector<int> result;
    std::merge(arr1.begin(), arr1.end(), arr2.begin(), arr2.end(),
               std::back_inserter(result));
    return result;
}

// Calculate the chunk size for each process
int get_chunk_size(int total_elements, int total_processes)
{
    assert(total_elements % total_processes == 0);
    return total_elements / total_processes;
}

void perform_merge_step(std::vector<int> &chunk, int chunk_size,
                        int rank_of_process, int number_of_process,
                        int number_of_elements, MPI_Status &status)
{
    int own_chunk_size = chunk_size;
    int received_chunk_size;

    for (int step = 1; step < number_of_process; step *= 2) {
        if (rank_of_process % (2 * step) != 0) {
            MPI_Send(chunk.data(), own_chunk_size, MPI_INT,
                     rank_of_process - step, 0, MPI_COMM_WORLD);
            break;
        }

        if (rank_of_process + step >= number_of_process)
            continue;

        received_chunk_size =
            (number_of_process >= rank_of_process + 2 * step)
                ? (chunk_size * step)
                : (number_of_elements - chunk_size * (rank_of_process + step));

        std::vector<int> chunk_received(received_chunk_size);
        MPI_Recv(chunk_received.data(), received_chunk_size, MPI_INT,
                 rank_of_process + step, 0, MPI_COMM_WORLD, &status);

        chunk = merge(chunk, chunk_received);
        own_chunk_size += received_chunk_size;
    }
}

int main(int argc, char *argv[])
{
    int number_of_elements;
    std::vector<int> data;
    double time_taken;
    MPI_Status status;

    MPI_Init(&argc, &argv);

    int number_of_process, rank_of_process;
    MPI_Comm_size(MPI_COMM_WORLD, &number_of_process);
    MPI_Comm_rank(MPI_COMM_WORLD, &rank_of_process);

    if (argc != 3) {
        std::cerr << "Usage: " << argv[0] << " <input_file> <output_file>\n";
        MPI_Finalize();
        return 1;
    }

    if (rank_of_process == 0) {
        std::ifstream file(argv[1]);
        if (!file) {
            std::cerr << "Error opening input file.\n";
            MPI_Abort(MPI_COMM_WORLD, 1);
        }

        char eol;
        file >> number_of_elements >> eol;
        data.resize(number_of_elements);
        for (int &elem : data) {
            file >> elem;
        }
        file.close();
    }

    MPI_Bcast(&number_of_elements, 1, MPI_INT, 0, MPI_COMM_WORLD);

    int chunk_size = get_chunk_size(number_of_elements, number_of_process);
    std::vector<int> chunk(chunk_size);

    if (rank_of_process == 0) {
        MPI_Scatter(data.data(), chunk_size, MPI_INT, chunk.data(), chunk_size,
                    MPI_INT, 0, MPI_COMM_WORLD);
    } else {
        MPI_Scatter(nullptr, 0, MPI_INT, chunk.data(), chunk_size, MPI_INT, 0,
                    MPI_COMM_WORLD);
    }

    MPI_Barrier(MPI_COMM_WORLD);
    time_taken = -MPI_Wtime();

    quicksort(chunk, 0, chunk_size);

    perform_merge_step(chunk, chunk_size, rank_of_process, number_of_process,
                       number_of_elements, status);

    time_taken += MPI_Wtime();

    if (rank_of_process == 0) {
        std::ofstream outfile(argv[2]);
        for (const auto &num : chunk) {
            outfile << num << " ";
        }
        outfile << std::endl;
        outfile.close();

        std::cout << "Quicksort " << number_of_elements << " ints on "
                  << number_of_process << " procs: " << time_taken << " secs\n";
    }

    MPI_Finalize();
    return 0;
}
