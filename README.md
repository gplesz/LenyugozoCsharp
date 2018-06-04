# LenyugozoCsharp
A NetAcademia lenyűgöző C# tanfolyam kiegészítő kódtára

- több alkalomra szétszedtük a tanfolyamot
  - Rasberry PI használata C# nyelven, led villogtatás (ma)
  - Saját csevegőrobot készítése saját gépen (botframework-simulator használatával)
  - csevegőrobot felhőbe telepítése (Azure) és összekötjük a Skype és a Facebook Messenger platformokkal.
  
  facebook: [dotnet cápák](https://www.facebook.com/groups/dotnetcapak)

# Telepítések

- Visual Studio Code + Git telepítése ([innen](https://code.visualstudio.com/))
  alapértelmezett beálításokkal, egyedül a szerkesztőnél kell beállítani a VIM helyett például a VisualStudioCode-ot.

- fritzig: rajzolóprogram ([innen tudjuk letölteni](http://fritzing.org/download/))
  Válasszuk a **No Donation**-t az ingyenes letöltéshez. Letöltés után a .zip archívumot tömörítsük ki valahová, ott tudjuk majd elindítani.

- DotNet környezet telepítése [innen](https://www.microsoft.com/net/download/windows)
  Letöltjük és futtatjuk a telepítőt, alapértelmezett beállításokkal.
  
- Docker telepítése [innen](https://store.docker.com/editions/community/docker-ce-desktop-windows)
 letöltjük, telepítjük.
 **Fontos:**
  - csak adminisztrátor felhasználóként telepítsük, 
  - többször újraindíthatja a gépet. 
  - HyperV-t engedélyezteti első induláskor

- SSH kliens (Windows-hoz: a Putty-ot fogjuk használni, [innen letölthető](https://www.chiark.greenend.org.uk/~sgtatham/putty/latest.html))

- zenmap: ezzel keressük meg a Raspberry-nket a hálózatunkon [innen](https://sourceforge.net/projects/nmap.mirror/?source=typ_redirect).

- Visual Studio Extensions:
  C#, Docker
  (Baloldalon az alsó ikon, és a keresőmezőbe: C#/Docker, **Install** gomb majd **Reload** gomb)

# Tematika

## Raspberry PI

### Megtervezzük, hogy mit is csinálunk? 

A Fritzing használatával kapcsolási rajzot készítünk a megvalósításhoz. Akit az elektronikai kérdések érdekelnek, az [ezen az ingyenes tanfolyamon](https://app.netacademia.hu/Tanfolyam/ELAI-I-az-elektronika-alapismeretei-i) tud egy kicsit jobban elmélyülni.

- A Raspberry GPIO az a felület, ahol elektronikai eszközöket tudunk a számítógéphez csatlakoztatni.
- A GPIO a tápfeszültség csatlakozóval szemben lévő oldalon van, két sor tüske.
- A külső oldalán fogunk két tüskét tüskét használni.
- (Ha felénk áll a tápfeszültség csatlakozó, akkor) Balról a harmadik tüske a földpont.
- A hatodik pedig az áramkörünk másik végpontja: ezt tudjuk PIN01 névvel megszólítani.
- Egy ledet és egy 650 Ohm-os ellenállást kötünk sorba erre áramkörre.

![Kapcsolási ábra](pics/kapcsolas.png)

Egy [ingyenes tanfolyam](https://app.netacademia.hu/Tanfolyam/rpifree-bevezetes-a-raspberry-pi-es-a-beagyazott-szoftverfejlesztes-kepzesbe) a Raspberry lelkivilágáról.

## Saját alkalmazás készítése

### Első működő verzió (csak válaszol)

```
md rpi.server
cd rpi.server
dotnet new webapi
```
### Csomagolás Docker segítségével
- A Dockerfile-ban leírjuk, hogy hogy épül fel a csomag
- build.bat-ban elkészítjük a "programot", ami előállítja és felküldi a docker hub-ra az egészet
- készítünk egy Docker Hub repo-t, ahova kerül a docker csomag, és ahonnan lehúzzuk a Raspberry-re ([ezaz](https://hub.docker.com/r/gplesz/rpi.server/)

### Kapcsolódunk a raspberry-hez
- ugyanazon a hálózati eszközön van, mint mi
- az IP címünket megkeressük IPConfiggal
  ```
  Ethernet adapter Ethernet 2:

   Connection-specific DNS Suffix  . :
   Link-local IPv6 Address . . . . . : fe80::84af:ad94:47f1:242d%12
   IPv4 Address. . . . . . . . . . . : 192.168.1.128
   Subnet Mask . . . . . . . . . . . : 255.255.255.0
   Default Gateway . . . . . . . . . : 192.168.1.1  
  ```
- zenmap-pel kiadjuk a parancsot: 
  ```
  nmap -sn 192.168.1.128/24
  ```
ahol a mi címünk: 192.168.1.128
- putty-tyal az így megszerzett címre behívunk SSH-val
  név: pi
  jelszó: raspberry

### telepítjük a docker-t a Raspberry-re
  ```
  curl -sSL https://get.docker.com | sh
  ```

### telepítjük az alkalmazást

```
sudo docker pull gplesz/rpi.server
sudo docker run -p 5000:5000 gplesz/rpi.server
```

de ez a második parancs nem éri el a **gpio**-t a dockerből, így ezt egy külön paraméterrel engedélyezni kell. Vagyis, a jó alkalmazás indítás a következő:

```
sudo docker run -p 5000:5000 --device /dev/gpiomem gplesz/rpi.server
```

vágólapról odamásolni a putty konzolba jobb egér kattintással lehet

### LED villogtatás

Egy [nuget csomagot](https://www.nuget.org/packages/Unosquare.Raspberry.IO/) használunk:
```
dotnet add package Unosquare.Raspberry.IO --version 0.14.0
```

# ajánlott tanfolyamok
- [Programozási alapismeretek C# nyelven](https://app.netacademia.hu/Tanfolyam/CsharpFree-programozasi-alapismeretek-c-nyelven)
- [elektronikai ismeretek](https://app.netacademia.hu/Tanfolyam/ELAI-I-az-elektronika-alapismeretei-i)
- Egy [Raspberry bevezetés](https://app.netacademia.hu/Tanfolyam/rpifree-bevezetes-a-raspberry-pi-es-a-beagyazott-szoftverfejlesztes-kepzesbe) a Raspberry lelkivilágáról.
- [Hálózati alapismeretek](https://app.netacademia.hu/Tanfolyam/HA-halozati-alapismeretek)


## Saját csevegőrobot készítése

Áttekintő ábra:

```
+------------------+
| Skype            |
|                  | +------------------------------------>+
|                  |                                       v
|                  |
+------------------+                          +----------------------------+
                                              |                            |
+------------------+                          |   Internetes               |            +-----------------------+
| Messenger        |                          |   csevegőrobot             | +------->  |  Csevegőrobot         |
|                  +----------------------->  |   csatornaszolgáltatás     |            |                       |
|                  |                          |                            |            |                       |
|                  |                          |                            |  <------+  |                       |
+------------------+                          |                            |            +-----------------------+
                                              |                            |
+------------------+                          |                            |
| Teams chat       | +----------------------> |                            |
|                  |                          +-----------------+----------+
|                  |                                            ^
|                  |                                            |
+------------------+                                            |
                                                                |
+------------------+                                            |
| Slack            |                                            |
|                  | +----------------------------------------->+
|                  |
|                  |
+------------------+
```


érdemes [a legutolsó .NET Core SDK-t](https://www.microsoft.com/net/download/windows) használni.

Letöltés és telepítés után használható is.

```
md bot.server
cd bot.server
dotnet new webapi
```

[A .NET Core és az ASP.NET Core újdonságai](https://blogs.msdn.microsoft.com/webdev/2018/02/27/asp-net-core-2-1-https-improvements/)

Ha valakinek a Developer Certificate-et nem fogadja el a gépe, akkor ezt kell mondania:

```
dotnet dev-certs https --trust
```

Ahhoz, hogy programozási környezetünk legyen a saját gépünkön, telepítjük a [Bot Framework Emulator](https://github.com/Microsoft/BotFramework-Emulator)-t, [innen](https://github.com/Microsoft/BotFramework-Emulator/releases).

[Összefoglaló cikk](https://docs.microsoft.com/hu-hu/azure/bot-service/bot-service-debug-emulator?view=azure-bot-service-3.0) a rendszerről.

[rövid leírás](https://hu.wikipedia.org/wiki/HTTP) a HTTP protokolról

A Bot Framework szabályai:
- a kérést POST üzenetként kapja az alkalmazásunk.
- a válaszunknak HTTP OK kódnak kell lennie

A kommunikációhoz telepítjük a [következő nuget csomagot](https://www.nuget.org/packages/Microsoft.Bot.Connector.AspNetCore):

```
dotnet add package Microsoft.Bot.Connector.AspNetCore --version 2.0.1.7
```

Ahhoz, hogy a csevegőrobotunkból elérjük a raspberry-n futó alkalmazást, RestApi hívást kell csinálni a [RestSharp]((http://restsharp.org/)) nevű [nuget csomaggal](https://www.nuget.org/packages/RestSharp).

```
dotnet add package RestSharp --version 106.3.0
```
