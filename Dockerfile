FROM kalilinux/kali-last-release

RUN apt update && \
    apt install -y inetutils-telnetd telnetd && \
    apt clean

# Adiciona um usuário para login via telnet (senha: player)
RUN useradd -m -s /bin/bash player && \
    echo "player:player" | chpasswd

RUN usermod -aG sudo player

# Habilita o serviço telnet no inetd
RUN echo "telnet  stream  tcp  nowait  root  /usr/sbin/tcpd  /usr/sbin/telnetd" >> /etc/inetd.conf

EXPOSE 23

CMD ["/usr/sbin/inetutils-inetd","-d"]
