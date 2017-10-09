using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using GVP.MCL.Enhanced;

namespace DFARMR_IVR1
{
    [Serializable]
    public class WebServiceResult
    {
        public const string COMPONENT = "component";
        public const string QUALITY = "quality";
        private Queue<KeyValuePair<string, string>> results;
        private OrderedDictionary componentResults;
        private OrderedDictionary qualityResults;
        private string tank;
        private string pickupDate;
        
        public WebServiceResult() : this(false)
        {
        }

        public WebServiceResult(bool expandedResults)
        {
            if (expandedResults)
            {
                this.componentResults = new OrderedDictionary(20);
                this.qualityResults = new OrderedDictionary(20);
            }
            else
            {
                this.results = new Queue<KeyValuePair<string, string>>(20);
            }
        }

        public WebServiceResult(int initialSize) : this(false, initialSize)
        {
        }

        public WebServiceResult(bool expandedResults, int initialSize)
        {
            if (expandedResults)
            {
                this.componentResults = new OrderedDictionary(initialSize);
                this.qualityResults = new OrderedDictionary(initialSize);
            }
            else
            {
                this.results = new Queue<KeyValuePair<string, string>>(initialSize);
            }
        }

        public void Add(string category, string name, string value)
        {
            if (componentResults != null)
            {
                switch (category)
                {
                    case COMPONENT:
                        if (!componentResults.Contains(name))
                        {
                            componentResults.Add(name, value);
                        }
                        break;
                    case QUALITY:
                        if (!qualityResults.Contains(name))
                        {
                            qualityResults.Add(name, value);
                        }
                        break;
                    default:
                        if (name.ToUpper().Equals("TANK"))
                            tank = value;
                        if (name.ToUpper().Equals("PICKUP DATE"))
                        {
                            pickupDate = value;
                        }
                        break;
                }
            }
        }

        public void Add(string name, string value)
        {
            if (results != null)
            {
                results.Enqueue(new KeyValuePair<string, string>(name, value));
            }
        }

        public string GetPromptText()
        {
            return GetPromptText(null, null);
        }

        public string GetPromptText(string category, string type)
        {
            TTS ttsBreak = new TTS();
            ttsBreak.silence = 250;
            Prompt p = new Prompt();
            

            StringBuilder variablePromptText = new StringBuilder(200);

            if (String.IsNullOrEmpty(category))
            {
                while (results.Count > 0)
                {
                    KeyValuePair<string, string> result = results.Dequeue();

                    variablePromptText.Append(result.Key.Replace("PROTEIN", "PROTEEN"));
                    variablePromptText.Append(" !!\\pause=250\\ ");
                    switch (result.Key.ToUpper())
                    {
                        case "TEST DATE":
                            variablePromptText.Append(" !!\\tn=date\\ ");
                            break;
                        case "LABEL":
                            variablePromptText.Append(" !!\\tn=spell\\ ");
                            break;
                        default:
                            break;
                    }
                    variablePromptText.Append(result.Value).Replace("PROTEIN", "PROTEEN");
                    variablePromptText.Append(" !!\\tn=normal\\ ");
                    variablePromptText.Append(" !!\\pause=250\\ ");

                    continue;
                }
                variablePromptText.Append(" !!\\pause=500\\ ");
                return variablePromptText.ToString();
            }

            OrderedDictionary anOrderedDictionary;

            if (category.Equals(COMPONENT))
                anOrderedDictionary = componentResults;
            else
                anOrderedDictionary = qualityResults;

            if (anOrderedDictionary.Count == 0)
            {
                return variablePromptText.ToString();
            }

            if (String.IsNullOrEmpty(type))
            {
                // list all
                
                variablePromptText.Append("Tank !!\\pause=250\\ ");
                variablePromptText.Append(tank);
                variablePromptText.Append(" !!\\pause=250\\ Pickup date !!\\pause=250\\ ");
                variablePromptText.Append(pickupDate);
                variablePromptText.Append(" !!\\pause=250\\ ");
                
                int count = anOrderedDictionary.Count;
                int i = 1;

                foreach (DictionaryEntry anEntry in anOrderedDictionary)
                {
                        
                    variablePromptText.Append((string)anEntry.Key);
                    variablePromptText.Append(" !!\\pause=250\\ ");
                    variablePromptText.Append((string)anEntry.Value).Replace("PROTEIN", "PROTEEN"); 
                    variablePromptText.Append(" !!\\pause=250\\ ");
                    i++;
                }
                variablePromptText.Append(" !!\\pause=500\\ ");
            }
            else
            {
                if (anOrderedDictionary.Contains(type))
                {
                    variablePromptText.Append("Tank !!\\pause=250\\ ");
                    variablePromptText.Append(tank);
                    variablePromptText.Append(" !!\\pause=250\\ Pickup date !!\\pause=250\\ ");
                    variablePromptText.Append(pickupDate);
                    variablePromptText.Append(" !!\\pause=250\\ ");
                    variablePromptText.Append(type);
                    variablePromptText.Append(" !!\\pause=250\\ ");
                    variablePromptText.Append(((string)anOrderedDictionary[type])).Replace("PROTEIN","PROTEEN");
                    variablePromptText.Append(" !!\\pause=500\\ ");
                }
            }
            return variablePromptText.ToString();
        }

        public bool HasResults(string componentOrQuality, string type)
        {
            bool hasResultsForCategoryAndType = false;

            if (!String.IsNullOrEmpty(componentOrQuality))
            {
                if (componentOrQuality.Equals(COMPONENT))
                {
                    if (String.IsNullOrEmpty(type))
                    {
                        hasResultsForCategoryAndType = componentResults.Count > 0;
                    }
                    else
                    {
                        hasResultsForCategoryAndType = componentResults.Contains(type);
                    }
                }
                if (componentOrQuality.Equals(QUALITY))
                {
                    if (String.IsNullOrEmpty(type))
                        hasResultsForCategoryAndType = qualityResults.Count > 0;
                    else
                        hasResultsForCategoryAndType = qualityResults.Contains(type);
                }
            }
            return hasResultsForCategoryAndType;
        }
    }
}