﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RoomsOfDoom
{
    public class Player : IHittable, ITile
    {
        public const int strength = 10;
        //Healing Potions, Time Crystals, Magic Scrolls
        public byte[] inventory = new byte[3] { 2, 2, 2 };
        List<IItem> activeItems;
        private int score = 0;

        public Player(int curHp = -1)
        {
            MaxHP = 100;
            if (curHp == -1)
                currentHP = MaxHP;
            else if (curHp > MaxHP)
                currentHP = MaxHP;
            else
                currentHP = curHp;
            Alive = true;
            Multiplier = 1;
            activeItems = new List<IItem>();
        }

        public bool Hit(int damage)
        {
            CurrentHP -= damage;
            return !Alive;
        }

        public bool Alive
        {
            get;
            private set;
        }

        int currentHP;
        public int CurrentHP
        {
            get
            {
                return currentHP;
            }
            set
            {
                currentHP = value;
                if (currentHP <= 0)
                    Alive = false;
                if (currentHP > MaxHP)
                    currentHP = MaxHP;
            }
        }

        public int MaxHP
        {
            get;
            private set;
        }

        public char Glyph
        {
            get { return '☻'; }
        }

        public Point Location
        {
            get;
            set;
        }

        public bool Move(Direction direction, Pack enemies)
        {
            Point loc = new Point();
            switch (direction)
            {
                case Direction.Up:
                    loc = new Point(Location.X, Location.Y - 1);
                    if (loc.Y == 0)
                        return false;
                    break;
                case Direction.Down:
                    loc = new Point(Location.X, Location.Y + 1);
                    if (loc.Y == Arena.Height - 1)
                        return false;
                    break;
                case Direction.Left:
                    loc = new Point(Location.X - 1, Location.Y);
                    if (loc.X == 0)
                        return false;
                    break;
                case Direction.Right:
                    loc = new Point(Location.X + 1, Location.Y);
                    if (loc.X == Arena.Width - 1)
                        return false;
                    break;
            }
            foreach(Enemy enemy in enemies)
                if(enemy.Location == loc)
                {
                    Combat(enemy);
                    return true;
                }
            Location = loc;
            return true;
        }

        public int Multiplier
        {
            get;
            set;
        }

        public bool OP
        {
            get;
            set;
        }

        public void Combat(Enemy enemy)
        {
            if(OP)
            {
                Enemy[] enemies = (Enemy[])enemy.myPack.Enemies.ToArray().Clone();
                foreach (Enemy e in enemies)
                    if (e.Hit(strength * Multiplier))
                        score += e.name.Length;
            }
            else
                if (enemy.Hit(strength * Multiplier))
                    score += enemy.name.Length;
        }

        public void UpdateItems()
        {
            IItem[] itemList = (IItem[])activeItems.ToArray().Clone();
            foreach (IItem i in itemList)
            {
                if (i.Duration == 0)
                {
                    activeItems.Remove(i);
                    i.Finish(this);
                }
                i.Duration--;
            }
        }


        public bool UseItem(IItem item, Dungeon dungeon)
        {
            if (item.Id < 0 || item.Id > inventory.Length)
                return false;
            if (inventory[item.Id] <= 0)
                return false;
            inventory[item.Id]--;
            item.Use(this, dungeon);
            activeItems.Add(item);
            return true;
        }

        public void IncreaseScore(int i)
        {
            if (i < 0)
                throw new ArgumentOutOfRangeException();

            score += i;

            //Score wrapped to int.MinValue
            if (score < i)
                score = int.MaxValue;
        }

        public int GetScore
        {
            get { return score; }
        }

        public void SetItems(byte potion, byte crystal, byte scroll)
        {
            inventory[0] = potion;
            inventory[1] = crystal;
            inventory[2] = scroll;
        }

        public void AddPotion()
        {
            if(inventory[0] != 255)
                inventory[0]++;
        }

        public void AddCrystal()
        {
            if (inventory[1] != 255)
                inventory[1]++;
        }

        public void AddScroll()
        {
            if (inventory[2] != 255)
                inventory[2]++;
        }

        public int GetPotCount
        {
            get { return inventory[0]; }
        }

        public int GetCrystalCount
        {
            get { return inventory[1]; }
        }

        public int GetScrollCount
        {
            get { return inventory[2]; }
        }
    }
}
