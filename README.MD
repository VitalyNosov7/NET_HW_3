## Домашнее задание к Уроку 3. PLINQ и асинхронность: как работает, области применения:

Добавьте использование Cancellationtoken в код сервера, чтобы можно было правильно останавливать работу сервера.

---
## Решение:

Для решения задачи был реализованы следующие действия:
1. В классе Program добавлены поля (стр.10 - 11)
```
        private static CancellationTokenSource cts = new CancellationTokenSource();
        private static CancellationToken ct = cts.Token;
```
2. В методе Server(), добавлено проверка условия (стр. 92 -103)
```
        if (message.TextMessage == "Exit")
        {
              Console.WriteLine("Для завершения работы сервера нажмите Enter!");

              Console.ReadLine();

              cts.Cancel();

              Console.WriteLine("Сервер отключён!");

              Environment.Exit(0);
       }

```

После того, как пользователь вводит ключевое слово "Exit", код метода Server() (стр. 92 -103) завершает работу программы сервера.