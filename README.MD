## �������� ������� � ����� 3. PLINQ � �������������: ��� ��������, ������� ����������:

�������� ������������� Cancellationtoken � ��� �������, ����� ����� ���� ��������� ������������� ������ �������.

---
## �������:

��� ������� ������ ��� ����������� ��������� ��������:
1. � ������ Program ��������� ���� (���.10 - 11)
```
        private static CancellationTokenSource cts = new CancellationTokenSource();
        private static CancellationToken ct = cts.Token;
```
2. � ������ Server(), ��������� �������� ������� (���. 92 -103)
```
        if (message.TextMessage == "Exit")
        {
              Console.WriteLine("��� ���������� ������ ������� ������� Enter!");

              Console.ReadLine();

              cts.Cancel();

              Console.WriteLine("������ ��������!");

              Environment.Exit(0);
       }

```

����� ����, ��� ������������ ������ �������� ����� "Exit", ��� ������ Server() (���. 92 -103) ��������� ������ ��������� �������.