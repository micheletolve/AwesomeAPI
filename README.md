# AwesomeAPI

[![Language](https://img.shields.io/badge/language-C%23-green.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://mit-license.org/)
![GitHub repo size](https://img.shields.io/github/repo-size/micheletolve/AwesomeAPI)

### Technology Stack
![Language](https://img.shields.io/badge/-C%20Sharp-grey?style=for-the-badge&logo=csharp&logoWidth=20&logoColor=white&labelColor=blue)
![Framework](https://img.shields.io/badge/-DotNet5-grey?style=for-the-badge&logo=dotnet&logoWidth=20&logoColor=white&labelColor=blue)
![C#](https://img.shields.io/badge/-CSharp-grey?style=for-the-badge&logo=csharp&logoWidth=20&logoColor=white&labelColor=blue)

![Bootstrap](https://img.shields.io/badge/-Bootstrap-grey?style=for-the-badge&logo=bootstrap&logoWidth=20&logoColor=white&labelColor=blue)
![HTML](https://img.shields.io/badge/-HTML-grey?style=for-the-badge&logo=html5&logoWidth=20&logoColor=white&labelColor=blue)
![CSS](https://img.shields.io/badge/-CSS-grey?style=for-the-badge&logo=css3&logoWidth=20&logoColor=white&labelColor=blue)
![JSON](https://img.shields.io/badge/-JSON-grey?style=for-the-badge&logo=json&logoWidth=20&logoColor=white&labelColor=blue)
![Node.js](https://img.shields.io/badge/-Node.js-grey?style=for-the-badge&logo=node.js&logoWidth=20&logoColor=white&labelColor=blue)
![Angular](https://img.shields.io/badge/-Angular-grey?style=for-the-badge&logo=angularjs&logoWidth=20&logoColor=white&labelColor=blue)
![Javascript](https://img.shields.io/badge/-javascript-grey?style=for-the-badge&logo=javascript&logoWidth=20&logoColor=white&labelColor=blue)
![Typescript](https://img.shields.io/badge/-typescript-grey?style=for-the-badge&logo=typescript&logoWidth=20&logoColor=white&labelColor=blue)
![Markdown](https://img.shields.io/badge/-Markdown-grey?style=for-the-badge&logo=markdown&logoWidth=20&logoColor=white&labelColor=blue)

![Visual Studio Code](https://img.shields.io/badge/-Visual%20Studio%20Code-grey?style=for-the-badge&logo=visual-studio-code&logoWidth=20&logoColor=white&labelColor=blue)

![MacOS](https://img.shields.io/badge/-BigSur-grey?style=for-the-badge&logo=macos&labelColor=blue&logoWidth=20&logoColor=white)
![Linux](https://img.shields.io/badge/-Debian-grey?style=for-the-badge&logo=linux&labelColor=blue&logoWidth=20&logoColor=white)
![Windows](https://img.shields.io/badge/-Win10-grey?style=for-the-badge&logo=windows&labelColor=blue&logoWidth=20&logoColor=white)
![Docker](https://img.shields.io/badge/-Docker%20Ready-grey?style=for-the-badge&logo=docker&labelColor=blue&logoWidth=20&logoColor=white)

### Data can be stoered in
![PostgreSQL](https://img.shields.io/badge/-PostgreSQL-grey?style=for-the-badge&logo=postgresql&logoColor=black&labelColor=cyan&logoWidth=20)
![MySQL](https://img.shields.io/badge/-MySQL-grey?style=for-the-badge&logo=mysql&logoColor=black&labelColor=cyan&logoWidth=20)
![MSS](https://img.shields.io/badge/-MSSQLServer-grey?style=for-the-badge&logo=microsoftsqlserver&logoColor=black&labelColor=cyan&logoWidth=20)
![SQLite](https://img.shields.io/badge/-SQLite-grey?style=for-the-badge&logo=sqlite&logoColor=black&labelColor=cyan&logoWidth=20)

Just simply change in the file <i>Startup.cs</i> the section about the DbContext, ⚠️  remember to change the connectionsString in the <i>appsettings.json</i>.
```c#
serices.AddDbContext<AuthenticationsDbContext>(options =>
{
    var connectionString = Configuration.GetConnectionString("SqlLite");
    options.UseSqlite(connectionString, b => b.MigrationsAssembly("AwesomeAPI"));
    //options.UseNpgsql(connectionString, b => b.MigrationsAssembly("AwesomeAPI"));
    //options.UseSqlServer(connectionString, b => b.MigrationsAssembly("AwesomeAPI"));

});
```

### Versioning with
![Git](https://img.shields.io/badge/-Git-grey?style=for-the-badge&logo=git&labelColor=red&logoWidth=20&logoColor=white)
![GitHub](https://img.shields.io/badge/-GitHub-grey?style=for-the-badge&logo=github&labelColor=red&logoWidth=20&logoColor=white)

Can get the source easily cloning the repository via SSH, ⚠️ remember to restore package for first.
```bash
git clone git@github.com:micheletolve/AwesomeAPI.git
```

### License

This software is released under **MIT Licence**.


