using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ElevatorArea51
{
    enum SecurityLevel
    {
        Confidential,
        Secret,
        TopSecret
    }

    class Elevator
    {
        public int CurrentFloor { get; set; }
        public int TargetFloor { get; set; }
        public bool IsMoving { get; set; }

        public Elevator()
        {
            CurrentFloor = 0;
            TargetFloor = 0;
            IsMoving = false;
        }

        public void Run()
        {
            while (true)
            {
                if (CurrentFloor < TargetFloor)
                {
                    // Асансьора качва 1 етаж.
                    IsMoving = true;
                    for (int i = CurrentFloor; i < TargetFloor; i++)
                    {
                        CurrentFloor++;
                        Console.WriteLine("Elevator moving to floor " + GetFloorString(TargetFloor));
                        Thread.Sleep(1000);
                   
                        
                    }
                }
                else if (CurrentFloor > TargetFloor)
                {
                    // Асансьора сваля 1 етаж.
                    IsMoving = true;
                    for (int i = CurrentFloor; i > TargetFloor; i--)
                    {
                        CurrentFloor--;
                        Console.WriteLine("Elevator moving to floor " + GetFloorString(TargetFloor));
                        Thread.Sleep(1000);
                     
                       
                    }
                }

                IsMoving = false;
            }
        }
        //Наимеонавията на етажите
        public string GetFloorString(int floor)
        {
            if (floor == 0)
            {
                return "G";
            }
            else if (floor == 1)
            {
                return "S";
            }
            else if (floor == 2)
            {
                return "T1";
            }
            else if (floor == 3)
            {
                return "T2";
            }

            return "";
        }
    }
    class Agent
    {
        public SecurityLevel SecurityLevel { get; set; }
        public int CurrentFloor { get; set; }
        public int TargetFloor { get; set; }
        public bool IsPressingButton { get; set; }
        public string Name { get; set; }

        public Agent(SecurityLevel securityLevel, string name ,int currentFloor)
        {
            SecurityLevel = securityLevel;
            CurrentFloor = currentFloor;
            TargetFloor = currentFloor;
            Name = name;
            IsPressingButton = false;
        }

        public void Run()
        {
            while (true)
            {
                // Генериране на етаж за симулация
                Random random = new Random();
                TargetFloor = random.Next(4);

                //Натискане на бутона за извикване на асансьора
                if (random.Next(1) == 0)
                {
                    IsPressingButton = true;
                }
                else
                {
                    IsPressingButton = false;
                }

                // Ъпдейтдване на етажа.
                CurrentFloor = TargetFloor;

                Thread.Sleep(1000);
            }
        }
    }

    
    class ElevatorDoor
    {
        public bool IsOpen { get; set; }

        public ElevatorDoor()
        {
            IsOpen = false;
        }

        public bool SecurityCheck(int floor, SecurityLevel securityLevel)
        {
            if (securityLevel == SecurityLevel.Confidential && floor == 0)
            {
                // Confidential агентите достъпват само ground етажи
                return true;
            }
            else if (securityLevel == SecurityLevel.Secret && (floor == 0 || floor == 1))
            {
                // Secret агентите достъпват ground и secret етажи
                return true;
            }
            else if (securityLevel == SecurityLevel.TopSecret && (floor == 0 || floor == 1 || floor == 2 || floor == 3))
            {
                // Top-secret агенти достъпват всички етажи
                return true;
            }

            return false;
        }

        public void Open()
        {
            IsOpen = true;
            Console.WriteLine("Door opened");
        }

        public void Close()
        {
            IsOpen = false;
            Console.WriteLine("Door closed");
        }
    }

    
    class Button
    {
        public bool IsDisabled { get; set; }

        public Button()
        {
            IsDisabled = false;
        }

        public void Disable()
        {
            IsDisabled = true;
        }

        public void Enable()
        {
            IsDisabled = false;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            
            Elevator elevator = new Elevator();

            List<Agent> agents = new List<Agent>();
            agents.Add(new Agent(SecurityLevel.Confidential, "Agent Aleksandar Angelov",0));
            agents.Add(new Agent(SecurityLevel.Secret, "Agent Petko Petkov", 0));
            agents.Add(new Agent(SecurityLevel.TopSecret, "Agent Nikolay Pavlov", 0));

            
            ElevatorDoor door = new ElevatorDoor();

           
            List<Button> buttons = new List<Button>();
            for (int i = 0; i < 4; i++)
            {
                buttons.Add(new Button());
            }

            
            Thread elevatorThread = new Thread(new ThreadStart(elevator.Run));
            elevatorThread.Start();

            
            foreach (Agent agent in agents)
            {
                Thread agentThread = new Thread(new ThreadStart(agent.Run));
                agentThread.Start();
            }

            // Започване на симулацията
            while (true)
            {
                 string GetFloorString(int floor)
                {
                    if (floor == 0)
                    {
                        return "G";
                    }
                    else if (floor == 1)
                    {
                        return "S";
                    }
                    else if (floor == 2)
                    {
                        return "T1";
                    }
                    else if (floor == 3)
                    {
                        return "T2";
                    }

                    return "";
                }
                // Проверка дали някой агент натиска бутона за извикване на асансьора
                for (int i = 0; i < 4; i++)
                {
                    // Проверка дали някой агент на текущия етаж натиска бутона
                    bool buttonPressed = agents.Any(a => a.CurrentFloor == i && a.IsPressingButton);

                    if (buttonPressed)
                    {
                        Console.WriteLine("Button pressed on floor " + GetFloorString(i));

                        // Изключване на бутоните
                        foreach (Button button in buttons)
                        {
                            button.Disable();
                        }

                        
                        elevator.TargetFloor = i;

                        // Изчакваме асансьора да достигне искания етаж
                        while (elevator.IsMoving)
                       {
                            Thread.Sleep(1000);
                        }
                        Thread.Sleep(1000);
                        Console.WriteLine("Elevator arrived at floor " + GetFloorString(i));
                       

                        // Отново включваме бутоните 
                        foreach (Button button in buttons)
                        {
                            button.Enable();
                        }

                        // Проверка за правата на агентите
                        Agent agent = agents.FirstOrDefault(a => a.CurrentFloor == elevator.CurrentFloor && door.SecurityCheck(elevator.CurrentFloor, a.SecurityLevel));
                        if (agent != null)
                        {
                            Thread.Sleep(1500);
                            door.Open();
                        
                            Console.WriteLine(agent.Name + " (" + agent.SecurityLevel + ") allowed to exit the elevator on floor " + elevator.GetFloorString(elevator.CurrentFloor));
                            door.Close();
                           
                        }
                        else
                        {
                            // Няма агенти на текущия етаж които да имат правата да влязат в асансьора , проверяваме дали някой агент който е вече в асансьора има права да излезе от него.
                            Agent insideAgent = agents.FirstOrDefault(a => a.CurrentFloor == elevator.CurrentFloor && a.IsPressingButton && door.SecurityCheck(elevator.CurrentFloor, a.SecurityLevel));
                            if (insideAgent != null)
                            {
                                Thread.Sleep(1500);
                                // Агент в асансьора има нужните права, отваряме вратата и го пускаме да си ходи.
                                door.Open();
                               
                                Console.WriteLine(insideAgent.Name + " (" + insideAgent.SecurityLevel + ") allowed to exit the elevator on floor " + elevator.GetFloorString(elevator.CurrentFloor));
                                door.Close();
                           

                            }
                        }
                    }
                }
            }
        }
    }
}


