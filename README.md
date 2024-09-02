# Desbloqueio do modem PACE 5471 (Vivo/GVT)

Com a realização do desbloqueio, liberam-se funções para usar o modem em modo _bridge_, usar as interfaces SIP e etc.

## Desbloqueio
[Baixe a última versão do _firmware_ customizado](https://www.tripleoxygen.net/wiki/modem/v5471) e utilize a ferramenta _PaceFwUploader_ para facilitar a vida.

## Configurações

Para ativar a porta **ETHERNET** em modo _bridge_ (_geralmente a porta de cor vermelha atrás do aparelho_), conecte-se via SSH ao modem usando o seguinte comando:
```console
ssh -oKexAlgorithms=+diffie-hellman-group1-sha1 -oHostKeyAlgorithms=+ssh-dss -c 3des-cbc root@192.168.25.1
```

As credenciais de acesso são:
- Usuário: root
- Senha: toor

Execute a sequinte sequência de comandos:

![image](https://github.com/user-attachments/assets/9f8d90e8-340c-4cb2-8e1a-bb4a9dc64707)

- **Rede WiFi e o switch do modem devem utilizar o DHCP da rede principal**: conecte o cabo da internet em qualquer porta *amarela*.
- **Modem fornece a rede padrão (192.168.25.x) para os dispositivos conectados no mesmo (seja via cabo ou WiFi)**: conecte a internet na porta *vermelha*.

Reproduzi a seguir alguns comandos úteis, encontrados nesta [wiki](https://www.tripleoxygen.net/wiki/modem/v5471):

O modem contém uma ferramenta para editar suas configurações: CLI. Antes de executar quaisquer modificações abaixo, faça login via SSH e execute:
```console
cli
```

Execute os comandos desejados abaixo e logo após:
```console
fcommit
```

Algumas alterações necessitam de um _reboot_:
```console
reboot
```

Exemplo:
```console
cli
set Services_GvtConfig_AccessClass 4
fcommit
reboot
RunLevel
set Services_GvtConfig_AccessClass 4
```
Substitua o "4" pelo RunLevel desejado, podendo este ser 2 ou 4. Atenção! O modo 4 afetará os serviços de telefonia e TV.

### Ativar HPNA
```console
set HPNA_Enable 1
```
### Desativar TR069
```console
set Services_TR069_Enable 0
```
### Modulação xDSL
```console
set WANDSLInterfaceConfig_ModulationType mode
```
Onde mode: ADSL_multi, ADSL_G.dmt, ADSL_G.lite, ADSL_ANSI_T1.413, ADSL_G.dmt.bis, ADSL_2plus, ADSL_multi_AM, xDSL_multi, VDSL

### Porta WAN no modo PPPoE
```console
set WANEthernetInterface_1_VLANInterface_1_VID 0
set WANConnectionDevice_1_WANPPPConnection_Username "usuario"
set WANConnectionDevice_1_WANPPPConnection_Password "senha"
```
### Porta WAN no modo IP
```console
set WANEthernetInterface_1_VLANInterface_1_VID 0
set WANConnectionDevice_1_WANPPPConnection_Enable 0
set WANConnectionDevice_1_WANIPConnection_Enable 1
```
#### Para modo DHCP
```console
set WANConnectionDevice_1_WANIPConnection_AddressingType "DHCP"
```
#### Para modo IP estático
```console
set WANConnectionDevice_1_WANIPConnection_AddressingType "Static"
set WANConnectionDevice_1_WANIPConnection_IPAddress "10.0.0.10"
set WANConnectionDevice_1_WANIPConnection_SubnetMask "255.255.255.0"
set WANConnectionDevice_1_WANIPConnection_DefaultGateway "10.0.0.1"
```
### Ativar stack IPv6
```console
set Device_IPv6Enable 1
```
### SIP para Vono
```console
(Configuração basica)
set VoiceProfile_1_Enable 1
set VoiceProfile_1_NumberOfLines 1
set VoiceProfile_1_WANInterface 1
set VoiceProfile_1_Name 'Vono'
set VoiceProfile_1_SignalingProtocol 'SIP'
set VoiceProfile_1_SIP_ProxyServer 'vono.net.br'
set VoiceProfile_1_SIP_ProxyServerPort 5060
set VoiceProfile_1_SIP_ProxyServerTransport 'UDP'
set VoiceProfile_1_SIP_OutboundProxy 'vono.net.br'
set VoiceProfile_1_SIP_OutboundProxyPort 1571
set VoiceProfile_1_SIP_RegisterExpires 300
fcommit
```
#### Configurando a linha telefônica
```console
set VoiceProfile_1_Line_1_Enable 1
set VoiceProfile_1_Line_1_SIP_AuthUserName 'user_vono'
set VoiceProfile_1_Line_1_SIP_AuthPassword 'senha_vono'
fcommit
```
##### Configuração caso tenha duas contas Vono com linhas e números diferentes
```console
set VoiceProfile_1_NumberOfLines 2
set VoiceProfile_1_Line_1_Enable 1
set VoiceProfile_1_Line_1_SIP_AuthUserName 'user_vono_linha1'
set VoiceProfile_1_Line_1_SIP_AuthPassword 'senha_vono_linha1'
set VoiceProfile_1_Line_2_Enable 1
set VoiceProfile_1_Line_2_SIP_AuthUserName 'user_vono_linha2'
set VoiceProfile_1_Line_2_SIP_AuthPassword 'senha_vono_linha2'
fcommit
```
##### Exemplo de configuração caso tenha uma única linha, mas queira poder completar duas chamadas de dois aparelhos ao mesmo tempo, e quando receber chamadas, caso a linha 1 esteja ocupada, a ligação seja redirecionada para a linha 2
```console
set VoiceProfile_1_NumberOfLines 2
set VoiceProfile_1_Line_1_Enable 1
set VoiceProfile_1_Line_1_SIP_AuthUserName 'user_vono'
set VoiceProfile_1_Line_1_SIP_AuthPassword 'senha_vono'
set VoiceProfile_1_Line_2_Enable 1
set VoiceProfile_1_Line_2_SIP_AuthUserName 'user_vono$1'
set VoiceProfile_1_Line_2_SIP_AuthPassword 'senha_vono'
fcommit
```
### Permitir ping remoto/externo
```console
set WANConnectionDevice_1_Firewall_AllowRemotePing 1
```
### Backup do perfil SIP / telefone
Execute via SSH (fora do cli):
```console
/etc/bewan/scripts/backup-voice-profile
```
Será criado um arquivo com as configurações em /nvram/gvt/voice-backup.cli. Este comando está disponível apenas em versões >= 42004 do 42k Series.

### Alterar MTU
```console
set WANConnectionDevice_1_MaxMTUSize valor
```
Valor: entre 500 a 1500. Padrão = 1492.

### Habilitar SIP/telefone no modo bridge
```console
set WANConnectionDevice_2_Enable 1
```

### Extra
[Instruções](https://radzki.github.io/posts/Reverse-Engineering-Sagemcom-F@ST-5302/) para o modem SAGECOM F@ST 5302.
