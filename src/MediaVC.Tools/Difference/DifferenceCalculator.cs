using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;
using MediaVC.Difference.FileSegments;

namespace MediaVC.Tools.Difference
{
    public class DifferenceCalculator : IDifferenceCalculator
    {
        #region Constructor

        public DifferenceCalculator(IInputSource currentVersion, IInputSource newVersion)
        {

        }

        public DifferenceCalculator(IInputSource newVersion)
        {

        }

        #endregion

        #region Fields

        private readonly byte[] left = new byte[] { 0,1,2,3,4,5,6,7,8,9 };

        private readonly byte[] right = new byte[]
        {  
            // New block
            10, 13,

            // Deleted block
            // 0,1

            // Current version
            2,3,4,5,

            // Delete version

            // 
            12,14,20
        };

        #endregion

        #region Properties

        public IInputSource CurrentVersion { get; }

        public IInputSource NewVersion { get; }

        public IList<IFileSegmentInfo> Result { get; } = new List<IFileSegmentInfo>();

        public SynchronizationContext SynchronizationContext { get; set; } = SynchronizationContext.Current;

        #endregion

        #region Methods

        public ValueTask CalculateAsync() => CalculateAsync(default, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellation"></param>
        /// <exception cref="OperationCanceledException" />
        public async ValueTask CalculateAsync(CancellationToken cancellation, IProgress<float> progress)
        {
            cancellation.ThrowIfCancellationRequested();

            Synchronize(() => Result.Clear());

            if(CurrentVersion != null)
            {
                long leftPosition = 0, rightPosition = 0;
                IFileSegmentInfo segment = null;

                while(leftPosition < CurrentVersion.Length)
                {
                    Synchronize(() => progress?.Report((float) Math.Round((double) leftPosition / CurrentVersion.Length, 2)));
                    cancellation.ThrowIfCancellationRequested();

                    Func<long, bool> loopPredicate = index =>
                        index + leftPosition < CurrentVersion.Length &&
                        index + rightPosition < NewVersion.Length;

                    // Searching
                    for(long searchingIndex = 0; loopPredicate(searchingIndex); ++searchingIndex)
                    {
                        CurrentVersion.Position = searchingIndex + leftPosition;
                        NewVersion.Position = searchingIndex + rightPosition;

                        byte searchedByte = CurrentVersion.ReadByte();

                        if(searchedByte == NewVersion.ReadByte())
                        {
                            if(segment == null)
                            {
                                segment = new FileSegmentInfo
                                {
                                    Source = CurrentVersion,
                                    StartPosition = leftPosition + searchingIndex,
                                    EndPosition = leftPosition + searchingIndex
                                };
                            }
                            else if(segment.Source.Equals(CurrentVersion))
                            {
                                segment.EndPosition = leftPosition + searchingIndex;
                            }
                            else if(segment.Source.Equals(NewVersion))
                            {
                                Result.Add(segment);

                                segment = null;

                                leftPosition += searchingIndex;
                                rightPosition += searchingIndex;

                                break;
                            }
                        }
                        else
                        {
                            if(segment == null)
                            {
                                segment = new FileSegmentInfo
                                {
                                    Source = NewVersion,
                                    StartPosition = rightPosition + searchingIndex,
                                    EndPosition = rightPosition + searchingIndex
                                };
                            }
                            else if(segment.Source.Equals(CurrentVersion))
                            {
                                Result.Add(segment);

                                segment = null;

                                leftPosition += searchingIndex;
                                rightPosition += searchingIndex;

                                break;
                            }
                            else if(segment.Source.Equals(NewVersion))
                            {
                                segment.EndPosition = rightPosition + searchingIndex;
                            }
                        }
                    }
                }
            }
            else
            {
                Result.Add(new FileSegmentInfo
                {
                    Source = NewVersion,
                    StartPosition = 0,
                    EndPosition = NewVersion.Length - 1
                });
            }
        }

        private void Synchronize(Action workToDo)
        {
            if (workToDo != null)
                SynchronizationContext.Post(_ => workToDo(), null);
        }

        public static IEnumerable<IFileSegmentInfo> CalculateRemovedSegments(IEnumerable<IFileSegmentInfo> calculatedDifference)
        {

        }

        #endregion
    }
}
