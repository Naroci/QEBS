using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace QEBS.Base
{
    public class GameEventArgsBuilder
    {

        GameEventArgsBuilderContext context = null;

        public GameEventArgsBuilder()
        {
                
        }

        public GameEventArgsBuilder(GameEventArgsBuilderContext Context)
        {
            this.context = Context;
        }

        public void AddItemsFromCollection(GameEventArgsBuilderContext Context)
        {
            if (this.context == null){
                this.context = Context;
            }
            else
            {
                foreach (var collection in Context)
                {
                    if (!this.context.ContainsKey(collection.Key))
                        this.context.Add(collection.Key,collection.Value);
                    else{
                        foreach (var itm in collection.Value){
                            AddGameEventArgsToCollection(collection.Key, itm.Key, itm.Value);
                        }
                    
                    }
                }
            }
          
        }

        public void CreateNewContext()
        {
            this.context = new GameEventArgsBuilderContext();
        }

        public  GameEventArgsBuilderContext GetCurrentContext()
        {
            return this.context;
        }

        public List<GameEventArgs> GetResult()
        {
            List<GameEventArgs> result = new List<GameEventArgs>(); 
            if (context != null && context.Values != null)
            {
                int collectionIndex = 0;
                SortContext();
                foreach ( var collection in context.Values.OrderBy(x=>x.GetOrderNumber()))
                {
                    if (collection.Values != null && collection.Values.Count > 0)
                    {
                    var items =  collection.Values.ToList();
                        for (int i = 0; i < items.Count; i++)
                        {
                            items[i].OverrideTimeStamp(new DateTime(collectionIndex + i));
                            result.Add(items[i]);
                        }    
                    }
                collectionIndex++;
                }
            }
           
            return result;
        }

        // AddForward wenn die Collection an erster Stelle gesetzt werden soll und alle bisherigen Elemente nachfolgen sollen.
        public void CreateCollection(string Identifier, bool AddForward = false)
        {
            
            if (this.context == null)
                this.context = new GameEventArgsBuilderContext();


            if (!CollectionExists(Identifier))
            {
                if (AddForward == true && this.context.Count > 0 )
                {
                    foreach (var collection in this.context){
                        collection.Value.SetOrderNumber(collection.Value.GetOrderNumber() + 1);
                    }

                    //this.context.Add(Identifier,new GameEventArgsCollection(0));
                }
                SortContext();
                var lastIndex = this.context.Count > 0 ? this.context.Values.Last().GetOrderNumber() : -1; 
                this.context.Add(Identifier,new GameEventArgsCollection( AddForward == true ? 0 : lastIndex + 1)); 
                SortContext();
            }
        }

        public bool CollectionExists(string Identifier){
            if (context == null)
                return false;

            return this.context.ContainsKey(Identifier);
        }

        private void SortContext(){
            this.context.Values.OrderBy(x=>x.GetOrderNumber());
        }

        public void ClearCollecion()
        {
            this.context.Clear();
            this.context = new GameEventArgsBuilderContext();
        }

        public void AddGameEventArgsToCollection(string CollectionName,string ItemIdentifier, GameEventArgs ItemToadd)
        {
            if (this.context != null && this.context.ContainsKey(CollectionName))
            {
                if (!context[CollectionName].ContainsKey(ItemIdentifier))
                    context[CollectionName].AddItem(ItemIdentifier,ItemToadd);
                else   
                     context[CollectionName][ItemIdentifier] = ItemToadd;
            }
            else
            {
                CreateCollection(CollectionName);
                context[CollectionName].AddItem(ItemIdentifier,ItemToadd);
            }
        }

        public void SetEventArgsItemFromCollection(string CollectionName, string EventArgsIdentifier, GameEventArgs Item)
        {
               if (this.context != null && this.context.ContainsKey(CollectionName) && context[CollectionName].ContainsKey(EventArgsIdentifier))
                   context[CollectionName].SetItem(EventArgsIdentifier,Item);
        }

        public T GetGameEventArgsItemFromCollectionByIdentifier<T>(string CollectionName,string EventArgsIdentifier) where T : GameEventArgs {
            if (this.context != null && this.context.ContainsKey(CollectionName))
                return (T)context[CollectionName].GetItem(EventArgsIdentifier);

            return null;
        }
    }

    public class GameEventArgsCollection : Dictionary<string,GameEventArgs>
    {
        public GameEventArgsCollection(){

        }

        public GameEventArgsCollection(int Order){
            this.orderNum = Order;
        }

        private int orderNum = 0;
        public int GetOrderNumber(){
            return this.orderNum;
        }

        public void SetOrderNumber(int Order){
            this.orderNum = Order;
        }

        public GameEventArgs GetItem(string Identifier)
        {
            if (ContainsKey(Identifier))
                return this[Identifier];

            return null;
        }

        public void SetItem(string Identifier, GameEventArgs Item)
        {
            if (this.ContainsKey(Identifier))
                this[Identifier] = Item;
        }


        public void AddItem(string Identifier, GameEventArgs Item)
        {
            this.Add(Identifier,Item);
        }
    }

    public class GameEventArgsBuilderContext : Dictionary<string,GameEventArgsCollection>
    {

    }
}
