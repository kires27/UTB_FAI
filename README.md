# git
## inspect branch state:
`git log --oneline --graph --decorate --all -20`

## .gitignore
If file already tracked, .gitignore does nothing. Remove from index:
`git rm --cached path/to/file`

or do for whole repo:
```sh
git rm -r --cached .
git add .
git commit -m ""
```

## workflow 
### Work on dev
```sh
git switch develop
git pull
git add .
git commit -m "Describe your changes"
git push
```

### Release to prod
```sh
git switch prod
git pull
git merge --ff-only develop
git push
```

## upstream
Check tracking
`git branch -vv`
setup upstream (usually on first push)
`git push -u origin prod`
