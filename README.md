## Mixi2Discord

mixiのつぶやきをDiscordに通知するAzure Funcions。

## 使い方<a name="使い方"></a>

Azureへデプロイした後、アプリケーション設定にて下記項目を設定する必要があります。

|設定名|記載する内容|
|:--|:--|
|TableName|mixi2discordで使用するテーブル名。「mixi2discord」とかでおｋ|
|DiscordWebhookId|通知先のDiscordのWebhookId。Discordのサーバー設定から作成/取得できるやつ|
|DiscordWebhookToken|通知先のDiscordのWebhookToken。Discordのサーバー設定から作成/取得できるやつ|
|MixiEmail|mixiにログインする際に使用するメールアドレス|
|MixiPassword|mixiにログインする際に使用するパスワード|
|MixiClientProxy|mixiにログ神する際に使用するProxy設定文字列(xxx.xxx.xxx.xxx:port)|


上記設定後、5分おきにmixiのつぶやきを取得し、Discordに通知してくれるようになります。


### mixiへログインする際にProxyを使用する理由

mixiは普段からアクセスのないIPアドレスからログインされると「本人ですか？」という確認メールを送信してきます。Proxyを設定しない場合のAzure FunctionsのIPアドレスはそこそこバラバラであり、頻繁に確認メールが送信されてしまいます。これを抑止するため/送信元IPアドレスを固定するためにProxy設定を行っている。

## ローカル実行について

Mixi2Discord.Functionsプロジェクトをローカル実行する場合、```local.settings.json```ファイルを編集する必要があります。設定する内容は[デプロイ後の設定](#使い方)のものと同じです。


## Mixi2Discord.Testのテスト実行について

Mixi2Discord.Testプロジェクトにある幾つかのテストはそのままでは動きません。設定ファイルの更新、mixiからhtmlファイルをダウンロードしておく必要があります。


### app.configの設定

app.configに設定を記述する必要があります。設定する内容は[デプロイ後の設定](#使い方)のものと同じです。

### パーステスト対象のhtmlファイルのダウンロード・配置

htmlパーサーのテストは、そのままでは動きません。
パース対象となるhtmlファイルを手動でダウンロードし、適切なディレクトリに配置しておく必要があります。

#### root.htmlのダウンロード・配置

- ログインしていない状態で[mixiのトップページ](https://mixi.jp/)へアクセスし、htmlをダウンロードする
  - <https://mixi.jp/>
- ダウンロードしたファイルを「Mixi2Discord.Test/html/root.html」に配置する

#### recent_voice.htmlのダウンロード・配置

- ログインした状態で[最新のつぶやきページ](http://mixi.jp/recent_voice.pl)へアクセスし、htmlをダウンロードする
  - <http://mixi.jp/recent_voice.pl>
- ダウンロードしたファイルを「Mixi2Discord.Test/html/recent_voice.html」に配置する

また、それぞれのhtmlファイルについてVisualStudio上のプロパティから「出力ディレクトリにコピー」を「新しい場合はコピーする」に設定して下さい。


## Azure上での運用コスト目安

2017/10/17現在、Azure上での一日あたりのコストは0.2円程度です。
24時間ずっと5分おきに取得しにいってるので、夜中はTimerTriggerを起動しないように設定すればもっと安くなります。

