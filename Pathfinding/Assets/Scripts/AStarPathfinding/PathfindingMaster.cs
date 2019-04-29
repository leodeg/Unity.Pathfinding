using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using GridMaster;

namespace Pathfinding
{
    public class PathfindingMaster : MonoBehaviour
    {
        #region Variables
        [Header ("Thread Properties")]
        public int maxJobs = 3;

        public delegate void JobComplete (IEnumerable<GridMaster.Node> path);

        private List<Pathfinder> currentJobs;
        private List<Pathfinder> todoJobs;

        #endregion

        #region Properties

        private static PathfindingMaster singleton;

        public static PathfindingMaster Singleton
        {
            get
            {
                if (singleton == null)
                    singleton = new PathfindingMaster ();
                return singleton;
            }
        }

        #endregion

        #region Unity Methods

        private void Start ()
        {
            currentJobs = new List<Pathfinder> ();
            todoJobs = new List<Pathfinder> ();
        }

        private void Update ()
        {
            CompleteCurrentJobs ();
        }

        #endregion

        #region Jobs Methods

        public void CompleteCurrentJobs ()
        {
            if (currentJobs.Count > 0)
            {
                foreach (Pathfinder currentJob in currentJobs)
                {
                    if (currentJob.jobDone)
                    {
                        currentJob.NotifyComplete ();
                        currentJobs.Remove (currentJob);
                    }
                }
            }

            if (todoJobs.Count > 0 && currentJobs.Count < maxJobs)
            {
                Pathfinder job = todoJobs[0];
                todoJobs.RemoveAt (0);
                currentJobs.Add (job);

                Thread jobThread = new Thread (job.FindPath);
                jobThread.Start ();
            }
        }

        public void RequestPathfind (Node start, Node target, JobComplete completeCallback)
        {
            Pathfinder newJob = new Pathfinder (start, target, completeCallback);
            todoJobs.Add (newJob);
        }

        #endregion
    }
}