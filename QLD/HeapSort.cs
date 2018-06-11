using System.Collections.Generic;

namespace QLD
{
    /// <summary>
    /// Thuật toán Heap Sort
    /// </summary>
    class HeapSort
    {
        #region privateVar
        /// <summary>
        /// Size of Heap
        /// </summary>
        private int heapSize;
        /// <summary>
        /// Danh sách tạm chứa danh sách sẽ được sắp xếp
        /// </summary>
        private List<ThuaDat> output;
        #endregion
        #region Contruct
        /// <summary>
        /// Khởi tạo thuật toán         
        /// </summary>
        /// <param name="items">List truyền vào</param>
        public HeapSort(List<ThuaDat> items)
        {
            output = items;
        }
        #endregion
        #region privateFunction
        /// <summary>
        /// Khởi tạo cây nhị phân từ danh sách
        /// Hàm CreateHeap sẽ tạo heap từ dữ liệu đưa vào là mảng và kích thước mảng. 
        /// Sử dụng vòng lặp tính từ vị trí giữa mảng, CreateHeap sẽ lặp và sắp xếp 
        /// các phần tử để tạo nên heap thỏa mãn các tính chất cần thiết bằng hàm Heapify.
        /// </summary>
        /// <param name="arr">List arr</param>
        private void BuildHeap(List<ThuaDat> arr)
        {
            heapSize = arr.Count - 1;
            for (int i = heapSize / 2; i >= 0; i--)
            {
                Heapify(arr, i);
            }
        }
        /// <summary>
        /// function to swap elements
        /// </summary>
        /// <param name="arr">arr contain element</param>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        private void Swap(List<ThuaDat> arr, int x, int y)
        {
            ThuaDat temp = arr[x];
            arr[x] = arr[y];
            arr[y] = temp;
        }
        /// <summary>
        /// Hàm Heapify sẽ kiểm tra nút trái, nút phải và nút ngay tại vị 
        /// trí offset để tìm ra nút có giá trị lớn nhất. Trong trường hợp nút 
        /// có giá trị lớn nhất không phải nút ở vị trí offset, hàm sẽ đổi giá 
        /// trị 2 nút này và tiếp tục đệ quy Heapify từ vị trí nút có giá trị lớn 
        /// nhất để đi đến các nhánh tiếp theo để đảm bảo nút cha sẽ luôn có giá 
        /// trị lớn hơn các nút con.
        /// </summary>
        /// <param name="arr">List arr</param>
        /// <param name="index"> index</param>
        private void Heapify(List<ThuaDat> arr, int index)
        {
            int left = 2 * index;
            int right = 2 * index + 1;
            int largest = index;

            if (left <= heapSize && arr[left].CompareTo(arr[index]) > 0) 
            {
                largest = left;
            }

            if (right <= heapSize && arr[right].CompareTo(arr[largest]) > 0)
            {
                largest = right;
            }

            if (largest != index)
            {
                Swap(arr, index, largest);
                Heapify(arr, largest);
            }
        }
        /// <summary>
        /// Bắt đầu chạy Heap
        /// </summary>
        /// <param name="arr">List arr cần sort with Heap</param>
        private void PerformHeapSort()
        {
            BuildHeap(output);
            for (int i = output.Count - 1; i >= 0; i--)
            {
                Swap(output, 0, i);
                heapSize--;
                Heapify(output, 0);
            }            
        }
        #endregion
        #region publicFunction
        /// <summary>
        /// Lấy ra danh sách đã sắp xếp
        /// Danh sách sắp xếp là danh sách được gán khi khởi tạo
        /// </summary>
        /// <returns> Danh sách đã sắp xếp</returns>
        public List<ThuaDat> GetListSorted()
        {
            PerformHeapSort();
            return output;
        }
        /// <summary>
        /// Lấy ra danh sách đã sắp xếp
        /// Danh sách sắp xếp là danh sách được gán khi gọi hàm
        /// </summary>
        /// <param name="items"> Danh sách cần sắp xếp</param>
        /// <returns>Danh sách đã sắp xếp</returns>
        public List<ThuaDat> GetListSorted(List<ThuaDat> items)
        {
            output = items;
            PerformHeapSort();
            return output;
        }
        #endregion
    }
}
