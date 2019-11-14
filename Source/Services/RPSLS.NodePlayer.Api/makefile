BRANCH ?= jessica-node-player-fix
USERNAME ?= jldeen
USEREMAIL ?= jedeen@outlook.com
DEVSPACE ?= 'default/jessica-node-player-fix'

.PHONY: clean

clean:
	-make git-clean
	-make azds-clean

setup:
	-git config --global user.email $(USEREMAIL)
	-git config --global user.name $(USERNAME)

git-clean: 
	-git checkout master
	-git pull
	-git branch -d $(BRANCH)
	-git push origin -d $(BRANCH) && echo "$(BRANCH) successfully deleted"
	-git fetch --prune
	-git checkout -b $(BRANCH) origin/master
	-git push --set-upstream origin $(BRANCH)

azds-clean:
	-azds space remove -n $(DEVSPACE)
	