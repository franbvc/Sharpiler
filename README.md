# Sharpiler

## Status dos Testes
![git status](http://3.129.230.99/svg/franbvc/Sharpiler/)


# Info

## Dotnet Version -> 7.0.101

## Para rodar o projeto
* Dentro da pasta do projeto, rodar o comando `dotnet run "<ARGS>"`
    * Exemplo: `dotnet run "4+5"`

* Fora da pasta do projeto, rodar o comando `dotnet run --project <PATH_TO_PROJECT> "<ARGS>"`
    * Exemplo: `dotnet run --project "C:\Users\foo\Documents\GitHub\Sharpiler\Sharpiler1" "4+5"`

## Para gerar .exe
* Dentro da pasta do projeto, rodar o comando `dotnet build`
    * O arquivo .exe estará na pasta `bin\Debug\net7.0\Sharpiler1.exe`
    * Para rodar o .exe, rodar o comando `.\bin\Debug\net7.0\Sharpiler1.exe "<ARGS>"`
        * Exemplo: `.\bin\Debug\net7.0\Sharpiler1.exe "4+5"`


# Sobre o projeto

## Roteiro 1

### Diagrama Sintático
![Diagrama Sintático](./DS_1.png)

### EBNF
expr -> num {(+|-), num}  


## Roteiro 2

### Diagrama Sintático
![Diagrama Sintático_2](./DS_2.jpeg)

### EBNF 
expr -> term {(+|-), term}  
term -> num {(*|/), num} 
