using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Csharp
{
    [Serializable]
    class Person
    {
        string First_Name, Last_Name;

        public Person()
        {
            First_Name = "";
            Last_Name = "";
        }
        public Person(string First_Name, string Last_Name)
        {
            Set_All(First_Name, Last_Name);
        }
        public void Set_All(string First_Name, string Last_Name)
        {
            this.First_Name = First_Name;
            this.Last_Name = Last_Name;
        }
        public void Set_First_Name(string First_Name)
        {
            this.First_Name = First_Name;
        }
        public void Set_Last_Name(string Last_Name)
        {
            this.Last_Name = Last_Name;
        }
        public string Get_First_Name()
        {
            return this.First_Name;
        }
        public string Get_Last_Name()
        {
            return this.Last_Name;
        }


    }
    [Serializable]
    class BankAccount
    {
        public string Email, CardNumber, PinCode;
        double accountBalance;
        Person person = new Person();
        public BankAccount()
        {
            this.Email = "";
            this.CardNumber = "";
            this.PinCode = "0";
            this.accountBalance = 0.0;
        }
        public BankAccount(Person person, string Email, string Card_Number, string Pin_Code, double Initial_Balance)
        {
            Set_All(person, Email, Card_Number, Pin_Code, Initial_Balance);
        }

        public void Set_All(Person p1, string Email, string Card_Number, string Pin_Code, double Initial_Balance)
        {
            this.Email = Email;
            this.CardNumber = Card_Number;
            this.PinCode = Pin_Code;
            this.accountBalance = Initial_Balance;
            this.person = p1;
        }

        public void set_Email(string Email)
        {
            this.Email = Email;
        }
        public void set_Card_Number(string Card_Number)
        {
            this.CardNumber = Card_Number;
        }
        public void set_Pin_Code(string Pin_Code)
        {
            this.PinCode = Pin_Code;
        }
        public void set_accountBalance(double Initial_Balance)
        {
            this.accountBalance = Initial_Balance;
        }
        public void set_Person(Person person)
        {
            this.person = person;
        }
        public string get_Email()
        {
            return this.Email;
        }
        public string get_Card_Number()
        {
            return this.CardNumber;
        }
        public string get_Pin_Code()
        {
            return this.PinCode;
        }
        public double get_Initial_Balance()
        {
            return this.accountBalance;
        }
        public Person get_Person()
        {
            return this.person;
        }
    }
    [Serializable]
    class Bank
    {
        List<BankAccount> customers = new List<BankAccount>();
        public  int NumberOfCustomers = 0;
        static int Deposit_Tries = 1, Withdraw_Tries = 1;
        int Bank_Capacity;
        public Bank() {
            this.Bank_Capacity = 10;
        }
        public Bank(int Bank_Capacity) {
            this.Bank_Capacity = Bank_Capacity;
        }
        public void AddNewAccount(BankAccount obj)
        {
            if(NumberOfCustomers >= Bank_Capacity) {
                Console.WriteLine("Bank System is Full");
                return;
            }
            FileStream fs = new FileStream("data.txt", FileMode.Append, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();

            if (!(customers.Contains(obj)))
            {
                bf.Serialize(fs, obj);
                customers.Add(obj);
            }
            else
            {
                Console.WriteLine("This Account is already exist");
            }
            NumberOfCustomers += 1;
            fs.Close();
        }
        public BankAccount Search(string Card_Num, string Pin_Code)
        {
            foreach(var i in customers) {
                if(i.get_Card_Number() == Card_Num && i.get_Pin_Code() == Pin_Code) {
                    return i;
                }
            }

            BankAccount temp = new BankAccount();
            return temp;
        }
        public double CheckBalance(string Card_Num, string Pin_Code)
        {
            BankAccount returned = Search(Card_Num, Pin_Code);

            if (returned.get_Card_Number() == "-1")
            {
                return 0;
            }
            else
            {
                return returned.get_Initial_Balance();
            }
        }
        public void Save()
        {
            FileStream fs = new FileStream("data.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();

            foreach (BankAccount i in customers)
            {
                bf.Serialize(fs, i);
            }

            fs.Close();
        }
        public int Valid(BankAccount current, BankAccount obj)
        {
            if (current.get_Card_Number() == obj.get_Card_Number())
            {
                if (current.get_Pin_Code() == obj.get_Pin_Code())
                {
                    return 2;
                }
                return 1;
            }
            return 0;
        }

        public void Withdraw(BankAccount obj,  double Amount)
        {

            foreach (var cur in customers)
            {
                int to_check = Valid(cur, obj);

                if (to_check == 0)
                {
                    continue;
                }
                else if (to_check == 1)
                {

                    Console.WriteLine("Someone is trying to steal Your Money... try #" + Withdraw_Tries);
                    Withdraw_Tries += 1;
                }
                else
                {
                    if (Amount <= cur.get_Initial_Balance())
                    {

                        cur.set_accountBalance(cur.get_Initial_Balance() - Amount);
                        Console.WriteLine("Withdraw Done...Your current Balance is: " + cur.get_Initial_Balance());

                        Save();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Not enough Money... Work Harder :)");
                    }
                }
            }

            Console.WriteLine("Withdraw with " + Amount + " is not complete");
            return;
        }
        public void Deposit(BankAccount obj, double Amount)
        {

            foreach (var cur in customers)
            {
                int to_check = Valid(cur,obj);

                if (to_check == 0)
                {
                    continue;
                }
                else if (to_check == 1)
                {

                    Console.WriteLine("Someone is trying to give you some Money... try #" + Deposit_Tries);
                    Deposit_Tries += 1;
                }
                else
                {

                    cur.set_accountBalance(cur.get_Initial_Balance() + Amount);
                    Console.WriteLine("Deposit Done...Your current Balance is :" + cur.get_Initial_Balance());

                    Save();
                    return;
                }
            }
            Console.WriteLine("Deposit with " + Amount + " is not complete");
            return;
        }
        public List<BankAccount> Load()
        {
            List<BankAccount> Loaded = new List<BankAccount>();

            FileStream fs = new FileStream("data.txt", FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();

            while (fs.Position < fs.Length)
            {
                BankAccount obj = (BankAccount)bf.Deserialize(fs);
                Loaded.Add(obj);
            }

            fs.Close();
            return Loaded;
        }
        public bool IsBankUser(string Card_num, string Pin_Code) {
            bool found = false;
            foreach(var i in customers) {
                if(i.get_Card_Number() == Card_num && i.get_Pin_Code() == Pin_Code) {
                    found = true;
                }
            }
            return found;
        }
    }
    class Program
    {
        static void main()
        {
            
        }
    }
}
