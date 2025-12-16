eval (ssh-agent -c)
ssh-add ~/.ssh/pri

cd android && ./gradlew assembleDebug

ionic cap sync android

