using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCompiler.BackusNaur
{
    /// <summary>
    /// A queue that its create from a read only collection and can only dequeue, and its current read pointer can be pushes onto an state stack,
    /// </summary>
    public class StateDequeue<T>
    {
        public StateDequeue(IEnumerable<T> Data)
        {
            this.Data = Data.ToList().AsReadOnly();
            this.ReadPointer = 0;
        }
        private ReadOnlyCollection<T> Data;
        private int ReadPointer;
        private Stack<int> state = new Stack<int>();

        /// <summary>
        /// Push the current read pointer to the state stack
        /// </summary>
        public void PushState()
        {
            state.Push(ReadPointer);
        }

        /// <summary>
        /// Pop and assign the read pointer from the state stack
        /// </summary>
        public void PopState()
        {
            ReadPointer = state.Pop();
        }

        /// <summary>
        /// Pop an state from the stack without assigning it
        /// </summary>
        public void DropState()
        {
            state.Pop();
        }

        /// <summary>
        /// Gets weather the queue is empty
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return ReadPointer >= Data.Count;
            }
        }

        /// <summary>
        /// Read the next element without consuming it
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return Data[ReadPointer];
        }


        /// <summary>
        /// Read the next element without consuming it, if the read pointer is on the end of the queue, read the last data item
        /// </summary>
        /// <returns></returns>
        public T PeekOrAbsoluteLast()
        {
            return Data[Math.Min(ReadPointer, Data.Count - 1)];
        }


        /// <summary>
        /// Read the next last data element
        /// </summary>
        /// <returns></returns>
        public T AbsoluteLast()
        {
            return Data[Data.Count - 1];
        }

        /// <summary>
        /// Read an element from the queue
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            if (ReadPointer < Data.Count)
                return Data[ReadPointer++];
            else
                throw new InvalidOperationException("The queue is empty");
        }
    }
}
