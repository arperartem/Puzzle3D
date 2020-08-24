using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder
{
    private Dictionary<Vector2Int, Puzzle> dict = new Dictionary<Vector2Int, Puzzle>();

    private Puzzle finishPoint;
    private bool isFinished = false;
    private List<Puzzle> resultPath = new List<Puzzle>();
    private List<CurrentStep> currentSteps = new List<CurrentStep>();

    private List<Puzzle> stepedPoints = new List<Puzzle>();

    public List<Puzzle> Find(Puzzle start, Puzzle finish, Puzzle[] list)
    {
        finishPoint = null;
        isFinished = false;
        dict.Clear();
        resultPath.Clear();
        currentSteps.Clear();
        stepedPoints.Clear();

        for (int i = 0; i < list.Length; i++)
        {
            dict.Add(list[i].Cell, list[i]);
        }

        finishPoint = finish;

        currentSteps = Step(new CurrentStep(start, new List<Puzzle>()));

        while (currentSteps.Count > 0)
        {
            for (int i = currentSteps.Count - 1; i >= 0; i--)
            {
                currentSteps.AddRange(Step(currentSteps[i]));
                currentSteps.RemoveAt(i);
            }

            if (currentSteps.Count > 100000)
            {
                Debug.LogWarning("To many steps!!!");
                break;
            }
        }

        return resultPath;
    }
    private List<CurrentStep> Step(CurrentStep currStep)
    {
        if (isFinished)
        {
            return new List<CurrentStep>();
        }

        stepedPoints.Add(currStep.nextPoint);
        currStep.path.Add(currStep.nextPoint);

        List<Puzzle> neighbors = Neighbors(currStep.nextPoint);

        List<CurrentStep> currSteps = new List<CurrentStep>();


        for (int i = 0; i < neighbors.Count; i++)
        {
            if (IsEqualPoints(neighbors[i], finishPoint))
            {
                currStep.path.Add(neighbors[i]);
                isFinished = true;

                resultPath = currStep.path;
                return new List<CurrentStep>();
            }

            if (HasPoint(neighbors[i], stepedPoints))
            {
                continue;
            }

            if (currStep.path.Count > 1 && HasPoint(neighbors[i], currStep.path))
            {
                continue;
            }

            currSteps.Add(new CurrentStep(neighbors[i], new List<Puzzle>(currStep.path)));
        }

        return currSteps;
    }


    private bool IsEqualPoints(Puzzle a, Puzzle b) => a.Cell.x == b.Cell.x && a.Cell.y == b.Cell.y;

    private bool HasPoint(Puzzle point, List<Puzzle> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (IsEqualPoints(point, list[i]))
            {
                return true;
            }
        }
        return false;
    }

    private List<Puzzle> Neighbors(Puzzle point)
    {
        List<Puzzle> result = new List<Puzzle>();

        Puzzle neighbor;

        for (int i = 0; i < point.Directions.Length; i++)
        {
            if (point.Directions[i].Equals(Puzzle.Direction.Up))
            {
                if (dict.TryGetValue(new Vector2Int(point.Cell.x, point.Cell.y + 1), out neighbor))
                {
                    if (neighbor.Directions.Contains(Puzzle.Direction.Down))
                    {
                        result.Add(neighbor);
                    }
                }
            }

            if (point.Directions[i].Equals(Puzzle.Direction.Right))
            {
                if (dict.TryGetValue(new Vector2Int(point.Cell.x + 1, point.Cell.y), out neighbor))
                {
                    if (neighbor.Directions.Contains(Puzzle.Direction.Left))
                    {
                        result.Add(neighbor);
                    }
                }
            }

            if (point.Directions[i].Equals(Puzzle.Direction.Down))
            {
                if (dict.TryGetValue(new Vector2Int(point.Cell.x, point.Cell.y - 1), out neighbor))
                {
                    if (neighbor.Directions.Contains(Puzzle.Direction.Up))
                    {
                        result.Add(neighbor);
                    }
                }
            }

            if (point.Directions[i].Equals(Puzzle.Direction.Left))
            {
                if (dict.TryGetValue(new Vector2Int(point.Cell.x - 1, point.Cell.y), out neighbor))
                {
                    if (neighbor.Directions.Contains(Puzzle.Direction.Right))
                    {
                        result.Add(neighbor);
                    }
                }
            }
        }
        return result;
    }

    private class CurrentStep
    {
        public Puzzle nextPoint;
        public List<Puzzle> path;

        public CurrentStep(Puzzle nextPoint, List<Puzzle> path)
        {
            this.nextPoint = nextPoint;
            this.path = path;
        }
    }
}
