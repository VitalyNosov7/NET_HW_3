using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace task_1
{
    //     Добавим JSON сериализацию и десериализацию в наш класс.И протестируем его путем
    //    компоновки простого сообщения, сериализации и десериализации этого сообщения.
    public class Message
    {
        public string NickName { get; set; }
        public DateTime DateMessage { get; set; }

        public string TextMessage { get; set; }

        public string ToName { get; set; }


        public string MessageToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static Message MessageFromJson(string json)
        {
            return JsonSerializer.Deserialize<Message>(json) ?? new Message();
        }
    }
}
