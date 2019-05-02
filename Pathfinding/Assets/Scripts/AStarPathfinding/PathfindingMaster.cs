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
        public int maxThreads = 3;

        public delegate void JobComplete (IEnumerable<Node> path);

        private List<Pathfinder> currentJobs = new List<Pathfinder> ();
        private List<Pathfinder> todoJobs = new List<Pathfinder> ();

        #endregion

        #region Properties

        private static readonly PathfindingMaster singleton = new PathfindingMaster ();

        public static PathfindingMaster Singleton
        {
            get
            {
                return singleton;
            }
        }

        #endregion

        #region Unity Methods

        private void Update ()
        {
            Debug.Log ("PATHFINDING MASTER UPDATE");
            CompleteCurrentJobs ();
        }

        #endregion

        #region Jobs Methods

        public void CompleteCurrentJobs ()
        {
            Debug.Log ("CURRENT JOBS COUNT: [" + currentJobs.Count + "]");
            if (currentJobs.Count > 0)
            {
                Debug.Log ("COMPLETE CURRENT JOBS");
                foreach (Pathfinder currentJob in currentJobs)
                {
                    if (currentJob.jobDone)
                    {
                        Debug.Log ("JOB IS DONE");
                        currentJob.NotifyComplete ();
                        currentJobs.Remove (currentJob);
                    }
                }
            }

            Debug.Log ("TODO JOBS COUNT: [" + todoJobs.Count + "]");
            if (todoJobs.Count > 0 && currentJobs.Count < maxThreads)
            {
                Debug.Log ("START TODO JOBS");
                Pathfinder job = todoJobs[0];
                todoJobs.RemoveAt (0);
                currentJobs.Add (job);

                Thread jobThread = new Thread (job.FindPath);
                jobThread.Start ();
                Debug.Log ("THREAD START");
            }
        }

        public void RequestPathfind (Node start, Node target, JobComplete completeCallback)
        {
            Debug.Log ("NEW PATHFINDER REQUEST");
            Pathfinder newJob = new Pathfinder (start, target, completeCallback);
            todoJobs.Add (newJob);
            Debug.Log ("TODO JOBS COUNT AFTER ADDING REQUEST: [" + todoJobs.Count + "]");
        }

        #endregion
    }
}