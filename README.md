# XSLT Salary Processor 

Этот проект реализует тестовое задание: выполнение XSLT-преобразования Data1.xml/Data2.xml → Employees.xml, добавление атрибутов сумм и показ данных в Windows Forms UI.

## Структура проекта

```
transform/
├── Data/
│   ├── Data1.xml       (copied from ../Data/Data1.xml)
│   └── Data2.xml       (copied from ../Data/Data2.xml)
├── XSLT/
│   └── transform.xslt
├── Output/
│   └── Employees.xml
└── Src/
    ├── Program.cs
    └── MainForm.cs
```

## Как запустить

1. Убедитесь, что установлен .NET 6 SDK.
2. Откройте терминал в папке проекта (`../transform/Src/`) и выполните:
   - `dotnet run --project Program.cs`

Или откройте проект в Visual Studio 2022 и запустите.

<img width="809" height="602" alt="{72B017EA-D381-47AC-8774-C66449ABDBBF}" src="https://github.com/user-attachments/assets/2fc4abbd-a7b4-4afe-bedc-adb83e4fc52e" />

<img width="797" height="601" alt="{5822C1F4-9FE3-4724-87ED-16DD0E70ABA9}" src="https://github.com/user-attachments/assets/9b39e56c-3195-4a98-ad29-a6011ea54ecd" />



## Примечания

- Исходные файлы Data1.xml и Data2.xml были скопированы из путей:
  - ../Data/Data1.xml
  - ../Data/Data2.xml

- transform.xslt находится в папке `XSLT/transform.xslt`.

- Output/Employees.xml — примерный файл, приложение перезапишет его при выполнении.

