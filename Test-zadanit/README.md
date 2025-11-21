# XSLT Salary Processor (Test Task)

Этот проект реализует тестовое задание: выполнение XSLT-преобразования Data1.xml/Data2.xml → Employees.xml, добавление атрибутов сумм и показ данных в Windows Forms UI.

## Структура проекта

```
XsltTransformApp/
├── Data/
│   ├── Data1.xml    
│   └── Data2.xml       
├── XSLT/
│   └── transform.xslt
├── Output/
│   └── Employees.xml
├── XsltTransformApp.csproj
└── Src/
    ├── Program.cs
    └── MainForm.cs
```

## Как запустить

1. Убедитесь, что установлен .NET 6 SDK.
2. Откройте терминал в папке проекта (`XsltTransformApp`) и выполните:
   - `dotnet build`
   - `dotnet run --project XsltTransformApp.csproj`

Или откройте проект в Visual Studio 2022 и запустите.

## Примечания

- Исходные файлы Data1.xml и Data2.xml были скопированы из путей:
  - Data1.xml
  - Data2.xml

- transform.xslt находится в папке `XSLT/transform.xslt`.

- Output/Employees.xml — примерный файл, приложение перезапишет его при выполнении.

## Предложенные коммиты (последовательность)
1. Initial project structure
2. Add Data1.xml and Data2.xml
3. Add transform.xslt
4. Implement XSLT execution and XML processing logic
5. Add Windows Forms UI
6. Final polishing, README and .gitignore

