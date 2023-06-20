using System.Collections.Generic;
using System.Linq;

using Azure.AI.OpenAI;

using Domain;

namespace GenerativeAi.Functions.question;

public class DoctorPrompt
{
    public ChatMessage System
        => new(ChatRole.System,
               "You're acting like an expert in providing first aid. " +
               "Only answer questions based on the facts listed below in Dutch." +
               "If the facts below don't answer the question, say you don't know. " + 
               "Whatever you do, don't come up your own stuff and don't use information outside " + 
               "The user will ask questions in Dutch and you must reply in Dutch as well. " +
             @"""Your answer must abide by the following rules:
                 Your answer must at all times be valid json with the following format, even if the data is retrieved from only one source:
                    {
                        ""Answer"": ""The answer in your own words"",
                        ""Sources"": [
                            {
                                ""SourceId"": ""The id of the used source"",
                            }
                        ]
                    }""" +
               //"Whenever possible, try to respond as much as possible in steps like this: \r\n" + 
               //"1.: do this\r\n" + 
               //"2.: do that\r\n" + 
               //"3.: ...\r\n" + 
               "Try to be as descriptive as possible, but at all time avoid using information outside the provided information!" + 
               "Include as many data as possible. Some rows might contain IDs reffering to other articles " +
               @"""Your answer must abide by the following rules:
                 1: Your answer must be based solely on the provided sources. 
                 2: Every part of the answer must be supported only by the sources.
                 3: If the answer consists of steps, provide a clear bullet point list.
                 4: If you don't know the answer, just say that you don't know. Don't try to make up an answer.
                 5: NEVER provide questions in the answer.
                 6: Your answer must at all times be valid json with the following format, even if the data is retrieved from only one source:
                    {
                        ""Answer"": ""The answer in your own words"",
                        ""Sources"": [
                            {
                                ""SourceId"": ""The id of the used source"",
                            }
                        ]
                    }"""
);

    public static ChatMessage Question(string question,
                                       IEnumerable<SearchResult> searchResults)
    {
        var sources = string.Join(" ",
                                  searchResults.Select(r => $"sourceId: {r.ChunkId.Value} " +
                                                            $"content: {r.Content}"));
        return new ChatMessage(ChatRole.User,
                               $"sources: ```{sources}``` " +
                               $"question: ```{question}?```");
    }

    public ChatMessage Question(string question,
                                       string context)
    {
        //var sources = string.Join(" ",
        //                          searchResults.Select(r => $"sourceId: {r.ChunkId.Value} " +
        //                                                    $"content: {r.Content}"));
        return new ChatMessage(ChatRole.User,
                               $"sources: ```{context}``` " +
                               $"question: ```{question}?```");
    }
}