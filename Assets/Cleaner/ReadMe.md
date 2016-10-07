#Asset Cleaner


このアセットは、ゲームから参照されないオブジェクトをプロジェクトからunitypackageで固めて退避します。

退避するアセットは、マテリアル・スクリプト・シェーダー・モデル・プレハブ・ScriptableObject・他、全ての被参照ファイルが該当します。

#使い方

1.  残しておきたいSceneをBuildSettingsに登録します(※1)
2.  メニューバー>Assets>Delete Unused Assets from game を選択します。
3.  残したいアセットからチェックを外します
4.  Deleteボタンを押します
5.  ファイルのバックアップ後(※2)、削除されます。

※1 ゲームに含まれる判定は、BuildSettingsから参照されていること・Resoucesから参照されていること、参照されているアセットから参照されている事を元に計算しています。

※2 退避したファイルは、BackupUnusedAssets にunitypackageとして出力されます。必要なファイルが退避されている場合、このファイルから戻して下さい。

#既知の問題

*  jsは非対応です。無条件で残ります。
*  退避後にInvalidOperationExceptionが出る事があります。
*  削除後はUnityエディタを再起動しないと消えないエラーが表示される事があります。
*  空のフォルダが残る事があります。
*  ソースコードのストリップがうまく動かない場合は教えて下さい。
*  エディタの使用しているリソースが削除される事があります。相対パスでしている場合等。

#License

Asset Cleaner
Copyright (c) 2015 Tatsuhiko Yamamura

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php

このアセットはMITで提供しています。