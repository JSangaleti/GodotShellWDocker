# Integração Godot + Shell/Docker

## 1) Sobre
Projeto de terminal que implementa comandos de linguagem Shell (Unix, via Bash) em máquinas Docker na plataforma Godot, desenvolvido em C# (.NET).

Este projeto está sendo desenvolvimento com a intenção primária de uso no projeto [Cyber Resistance](https://github.com/JSangaleti/Cyber_Resistance), uma plataforma gamificada cujo objetivo é ensinar cibersegurança de forma didática, imersiva e entretiva.

## 2) Requisitos
Para rodar esta aplicação, é necessário ter instalado:

 1. Godot .NET v. 4.4 (e suas dependências necessárias para rodar uma aplicação);
 2. Docker;
 3. Construir um container Docker a partir do arquivo Dockerfile disponibilizado com o nome "player_machine" (instruções abaixo).
 
## 3) Construindo a aplicação 

### 3.1) Construindo o container
Para construir a imagem Docker a partir do arquivo Dockerfile, entre no diretório em que está o arquivo e execute o seguinte comando:
```
docker build -t player_machine .
```

Em seguida para criar o container:
```
docker create --name player_machine -p 5000:23 player_machine:latest 
```

> **Importante!** Certifique-se de que o servidor Docker em seu computador esteja rodando devidamente! A própria aplicação está responsável por iniciar e parar o container

### 3.2) Executando o projeto na Godot

Abra normalmente o projeto pela plataforma na Godot, utilizando a versão **4.4 .NET** (para evitar *bugs* de versionamento). pelo arquivo de cena ".tscn". Em seguida, clique em executar a aplicação normalmente.
> **Importante!** Atenção especial que a plataforma deve suportar **C#**, sendo a versão **.NET**! Caso contrário, a aplicação não irá funcionar!

> Além disso, os passos deverão ser seguidos na sequência descrita!

## Pacotes de Terceiros

 - [Minimalist Telnet Library](https://www.codeproject.com/Articles/19071/Quick-tool-A-minimalistic-Telnet-library): utilizada como intermediário entre a aplicação Godot e o Bash da máquina Docker; permite a emulação de terminal por uma sessão Telnet.