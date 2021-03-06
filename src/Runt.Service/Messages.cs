﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Runt.Core.Model;

namespace Runt.Service
{
    static class Messages
    {
        static readonly JsonSerializer _default = new JsonSerializer();
        static readonly JsonSerializer _ignoreDefault = new JsonSerializer()
        {
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        private static string Message(string type, string content)
        {
            return new JObject(
                new JProperty("type", new JValue(type)),
                new JProperty("data", new JRaw(content))
            ).ToString(Formatting.None);
        }

        internal static string Error(Exception e)
        {
            return Message("error", JsonConvert.SerializeObject(e));
        }

        public static string State(EditorState state, JsonSerializer serializer = null)
        {
            serializer = serializer ?? _default;
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
                serializer.Serialize(writer, state, typeof(EditorState));
            return Message("state", sb.ToString());
        }

        public static string StateUpdate(JObject diff)
        {
            return Message("state", diff.ToString(Formatting.None));
        }

        public static string Callback(int callback, Content content)
        {
            return Callback(callback, JObject.FromObject(content));
        }

        public static string Callback(int callback, JToken value)
        {
            return new JObject(
                new JProperty("type", new JValue("callback")),
                new JProperty("id", new JValue(callback)),
                new JProperty("data", value)
            ).ToString(Formatting.None);
        }

        internal static string Highlight(string contentId, JObject highlight)
        {
            JObject send = new JObject(
                new JProperty("cid", contentId),
                new JProperty("data", highlight)
            );
            return Message("highlight", send.ToString(Formatting.None));
        }
    }
}
