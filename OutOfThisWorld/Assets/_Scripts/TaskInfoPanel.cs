using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld.Debug;

namespace OutOfThisWorld.Player.HUD
{


    public class TaskInfoPanel : MonoBehaviour
    {

        public GameObject InfoWidgetPrefab;


        private Dictionary<string, TaskInfoWidget> currentTasks;
        private Dictionary<string, TaskNode> taskGraph;
        private TaskInfoWidget _defaultInfoWidget;

        private 

        // Start is called before the first frame update
        void Start()
        {
            currentTasks = new Dictionary<string, TaskInfoWidget>();
            taskGraph = new Dictionary<string, TaskNode>();

            InitializeTaskGraph();

            _defaultInfoWidget = InfoWidgetPrefab.GetComponent<TaskInfoWidget>();
            DebugUtility.HandleErrorIfNullGetComponent<TaskInfoWidget, GameObject>(_defaultInfoWidget, this, InfoWidgetPrefab);
            
            AddTaskInfo("Pick Up an Object (Left Click)");
        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool AddTaskInfo(string taskText)
        {
            if (currentTasks.ContainsKey(taskText)) return false;

            // Add the task to the panel
            GameObject newTaskInfo = Instantiate(InfoWidgetPrefab, transform);
            TaskInfoWidget widget = newTaskInfo.GetComponent<TaskInfoWidget>();
            DebugUtility.HandleErrorIfNullGetComponent<DroneInfoBar, GameObject>(widget, this, newTaskInfo);
            widget.setTaskText(taskText);

            currentTasks.TryAdd(taskText, widget);
            return true;
        }

        // Call this function anywhere where the task is completed 
        // Ex: CompleteTask("Create Second Drone (Left Click the ship with 5 RP)") is in PlayerController SpawnDrone
        public bool CompleteTask(string taskText)
        {
            // Remove the task in the panel.
            if (!currentTasks.TryGetValue(taskText, out var widget)) { return false; }
            if (!currentTasks.Remove(taskText)) { return false; }
            Destroy(widget.gameObject);

            // Add the next series of tasks
            if (taskGraph.TryGetValue(taskText, out TaskNode completedTask))
            {
                foreach (TaskNode nextTask in completedTask.NextTasks)
                {
                    AddTaskInfo(nextTask.TaskText);
                }
            }



            return true;
        }

        private void InitializeTaskGraph()
        {
            // Create Task
            TaskNode getObject = new TaskNode("Pick Up an Object (Left Click)");
            TaskNode dropObject = new TaskNode("Drop an object (Right Click)");
            TaskNode depositInShip = new TaskNode("Deposit item in ship (Left Click anywhere on the ship)");
            TaskNode accumulate5RP = new TaskNode("Accumulate 5 RP (Deposit items into the ship)");
            TaskNode createSecondDrone = new TaskNode("Create Second Drone (Left Click the ship to spend 5 RP)");
            TaskNode switchDrones = new TaskNode("Switch Drones (Tab)");
            TaskNode accumulate10RP = new TaskNode("Accumulate 10 RP");

            // Establish dependencies (what task will come after this task)
            getObject.AddNextTask(depositInShip);
            getObject.AddNextTask(dropObject);
            depositInShip.AddNextTask(accumulate5RP);
            accumulate5RP.AddNextTask(createSecondDrone);
            createSecondDrone.AddNextTask(switchDrones);
            createSecondDrone.AddNextTask(accumulate10RP);

            // Add nodes to the graph
            taskGraph.Add(getObject.TaskText, getObject);
            taskGraph.Add(dropObject.TaskText, dropObject);
            taskGraph.Add(depositInShip.TaskText, depositInShip);
            taskGraph.Add(accumulate5RP.TaskText, accumulate5RP);
            taskGraph.Add(createSecondDrone.TaskText, createSecondDrone);
            taskGraph.Add(switchDrones.TaskText, switchDrones);
            taskGraph.Add(accumulate10RP.TaskText, accumulate10RP);
        }


    }


    // Represents a graph of ordered tasks to do.
    public class TaskNode
    {
        public string TaskText;
        public List<TaskNode> NextTasks;

        public TaskNode(string taskText)
        {
            TaskText = taskText;
            NextTasks = new List<TaskNode>();
        }

        public void AddNextTask(TaskNode nextTask)
        {
            NextTasks.Add(nextTask);
        }
    }

}