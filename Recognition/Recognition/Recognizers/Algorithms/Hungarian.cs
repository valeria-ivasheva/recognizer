using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recognition.Recognizers.Algorithms
{
    public class Hungarian
    {
        private int numRows;
        private int numCols;

        private bool[,] primes;
        private bool[,] stars;
        private bool[] rowsCovered;
        private bool[] colsCovered;
        private List<List<double>> costs;

        public Hungarian(List<List<double>> theCosts)
        {
            costs = theCosts;
            numRows = costs.Count; 
            numCols = costs[0].Count;

            primes = new bool[numRows, numCols];
            stars = new bool[numRows, numCols];

            rowsCovered = new bool[numRows];
            colsCovered = new bool[numCols];
            for (int i = 0; i < numRows; i++)
            {
                rowsCovered[i] = false;
            }
            for (int j = 0; j < numCols; j++)
            {
                colsCovered[j] = false;
            }
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    primes[i, j] = false;
                    stars[i, j] = false;
                }
            }
        }

        public int[][] Execute()
        {
            SubtractRowColMins();

            this.FindStars();
            this.ResetCovered(); 
            this.CoverStarredZeroCols(); 

            while (!AllColsCovered())
            {
                List<int> primedLocation = PrimeUncoveredZero().ToList(); 

                if (primedLocation[0] == -1)
                {
                    this.MinUncoveredRowsCols(); 
                    primedLocation = PrimeUncoveredZero().ToList();
                }

                int primedRow = primedLocation[0];
                int starCol = this.FindStarColInRow(primedRow);
                if (starCol != -1)
                {
                    rowsCovered[primedRow] = true;
                    colsCovered[starCol] = false;
                }
                else
                {
                    this.AugmentPathStartingAtPrime(primedLocation);
                    this.ResetCovered();
                    this.ResetPrimes();
                    this.CoverStarredZeroCols();
                }
            }
            return this.StarsToAssignments();

        }
                
        private int[][] StarsToAssignments()
        {
            int[][] toRet = new int[numCols][];
            for (int j = 0; j < numCols; j++)
            {
                toRet[j] = new int[] {
                this.FindStarRowInCol(j), j
            };
            }
            return toRet;
        }
        
        private void ResetPrimes()
        {
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    primes[i, j] = false;
                }
            }
        }
                
        private void ResetCovered()
        {
            for (int i = 0; i < numRows; i++)
            {
                rowsCovered[i] = false;
            }
            for (int j = 0; j < numCols; j++)
            {
                colsCovered[j] = false;
            }
        }
        
        private void FindStars()
        {
            bool[] rowStars = new bool[numRows];
            bool[] colStars = new bool[numCols];

            for (int i = 0; i < numRows; i++)
            {
                rowStars[i] = false;
            }
            for (int j = 0; j < numCols; j++)
            {
                colStars[j] = false;
            }

            for (int j = 0; j < numCols; j++)
            {
                for (int i = 0; i < numRows; i++)
                {
                    if (costs[i][j] == 0 && !rowStars[i] && !colStars[j])
                    {
                        stars[i, j] = true;
                        rowStars[i] = true;
                        colStars[j] = true;
                        break;
                    }
                }
            }
        }

        private void MinUncoveredRowsCols()
        {
            double minUncovered = double.MaxValue;
            for (int i = 0; i < numRows; i++)
            {
                if (!rowsCovered[i])
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        if (!colsCovered[j])
                        {
                            if (costs[i][j] < minUncovered)
                            {
                                minUncovered = costs[i][j];
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < numRows; i++)
            {
                if (rowsCovered[i])
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        costs[i][j] = costs[i][j] + minUncovered;

                    }
                }
            }

            for (int j = 0; j < numCols; j++)
            {
                if (!colsCovered[j])
                {
                    for (int i = 0; i < numRows; i++)
                    {
                        costs[i][j] = costs[i][j] - minUncovered;
                    }
                }
            }
        }

        private int[] PrimeUncoveredZero()
        {
            int[] location = new int[2];

            for (int i = 0; i < numRows; i++)
            {
                if (!rowsCovered[i])
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        if (!colsCovered[j])
                        {
                            if (costs[i][j] == 0)
                            {
                                primes[i, j] = true;
                                location[0] = i;
                                location[1] = j;
                                return location;
                            }
                        }
                    }
                }
            }

            location[0] = -1;
            location[1] = -1;
            return location;
        }
        
        private void AugmentPathStartingAtPrime(List<int> location)
        {
            List<List<int>> primeLocations = new List<List<int>>(numRows + numCols);
            List<List<int>> starLocations = new List<List<int>>(numRows + numCols);
            primeLocations.Add(location);

            int currentRow = location[0];
            int currentCol = location[1];
            while (true)
            { 
                int starRow = FindStarRowInCol(currentCol);
                if (starRow == -1)
                {
                    break;
                }
                int[] starLocation = new int[] {
                starRow, currentCol};
                starLocations.Add(starLocation.ToList());
                currentRow = starRow;

                int primeCol = FindPrimeColInRow(currentRow);
                int[] primeLocation = new int[] {
                currentRow, primeCol
            };
                primeLocations.Add(primeLocation.ToList());
                currentCol = primeCol;
            }

            UnStarLocations(starLocations);
            StarLocations(primeLocations);
        }

        private void StarLocations(List<List<int>> locations)
        {
            for (int k = 0; k < locations.Count; k++)
            {
                List<int> location = locations[k];
                int row = location[0];
                int col = location[1];
                stars[row, col] = true;
            }
        }

        private void UnStarLocations(List<List<int>> starLocations)
        {
            for (int k = 0; k < starLocations.Count; k++)
            {
                List<int> starLocation = starLocations[k];
                int row = starLocation[0];
                int col = starLocation[1];
                stars[row, col] = false;
            }
        }

        private int FindPrimeColInRow(int theRow)
        {
            for (int j = 0; j < numCols; j++)
            {
                if (primes[theRow, j])
                {
                    return j;
                }
            }
            return -1;
        }

        private int FindStarRowInCol(int theCol)
        {
            for (int i = 0; i < numRows; i++)
            {
                if (stars[i, theCol])
                {
                    return i;
                }
            }
            return -1;
        }
        
        private int FindStarColInRow(int theRow)
        {
            for (int j = 0; j < numCols; j++)
            {
                if (stars[theRow, j])
                {
                    return j;
                }
            }
            return -1;
        }

        private bool AllColsCovered()
        {
            for (int j = 0; j < numCols; j++)
            {
                if (!colsCovered[j])
                {
                    return false;
                }
            }
            return true;
        }

        private void CoverStarredZeroCols()
        {
            for (int j = 0; j < numCols; j++)
            {
                colsCovered[j] = false;
                for (int i = 0; i < numRows; i++)
                {
                    if (stars[i, j])
                    {
                        colsCovered[j] = true;
                        break; 
                    }
                }
            }
        }

        private void SubtractRowColMins()
        {
            for (int i = 0; i < numRows; i++)
            { 
                double rowMin = double.MaxValue;
                for (int j = 0; j < numCols; j++)
                { 
                    if (costs[i][j] < rowMin)
                    {
                        rowMin = costs[i][j];
                    }
                }
                for (int j = 0; j < numCols; j++)
                { 
                    costs[i][j] = costs[i][j] - rowMin;
                }
            }

            for (int j = 0; j < numCols; j++)
            { 
                double colMin = double.MaxValue;
                for (int i = 0; i < numRows; i++)
                { 
                    if (costs[i][j] < colMin)
                    {
                        colMin = costs[i][j];
                    }
                }
                for (int i = 0; i < numRows; i++)
                { 
                    costs[i][j] = costs[i][j] - colMin;
                }
            }
        }
    }
}
