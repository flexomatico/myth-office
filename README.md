# Myth-Office
IT University of Copenhagen
Game World Design Semester Project
Spring 2023
## General Info
We will be using the following Unity version:

> Unity 2021.3.18

You can download it here: https://unity.com/releases/editor/archive

## Repository Policy
Hello team :)

For this project I would like to try out **trunk based development** with the least amount of friction possible. This means we do not have two protected branches (such as **main** and **dev**) like I remember many of us had in *Making Games* but only one **main** branch. The former does give more protection but I also remember some unnecessary duplicate work had to be done. 

With the approach I thought of, we only have one branch: **main**; and we keep it functional at all times by pushing our changes to it frequently and in small batches. It has weak protection enabled, meaning we cannot push changes directly to it, but have to do work in branches. When someone is done working on their feature, they generate a pull request, to merge their changes into **main**. It requires one person to approve the pull request, so theoretically the person generating the pull request can approve it at the same time. This allows us to get small changes into **main** quickly, without having to wait for someone else to approve. Should there be merge conflicts though, the merge can be aborted and brought to the attention of Felix or Aske, so we can have a look at it.

This should make development easy and quick, and as only Aske and Felix will do the main code-work, we shouldn't run into many conflicts. However, Tamara, Chris & Kris will also do work in form of level-building, putting together animations, dressing of scenes and more. To make sure we do not run into the problem of multiple people working on the same stuff simultaneously, we should work with branches. When you want to work on something, follow these steps. 

 1. *fetch* and *pull*, so that you are up to date with the latest changes.
 2. Create a branch off of **main**, naming it in this format: 

> **task-type/task-description**

some examples:

> feature/player-jump

> experimental/toon-shader

> bug-fix/collider-level1-fix

 3. Create a scene in Unity with the same *task-description*, you chose for your branch name.
 4. Do work in the scene, commit changes along the way.
 5. When finished, push your changes and create a pull request.
 6. Approve the merge yourself, to see if there are merge conflicts. If not: perfect! if so: tell Felix ;)
 7. When everything went well: delete the branch to keep the repo clean.

## Unity Scene Setup
Additionally, I would like to try out Unity's multi scene workflow. This means that we load multiple scenes simultaneously. The benefit of this can be that our workflow will be less cluttered. If, someone for example wants to create the office layout for Zeus, they can do so by following step 1. - 4. above. Then they can add the *Player* scene to their newly created scene, to have a controllable player to explore their creation. I hope that this way, scenes will stay small and tidy by only including things needed for working on a specific feature, even when we're further along in development.

Everyone has their own folder dedicated for one's own scenes. Additionally, we have a folder called *Main*. Here we store and work on scenes that make up the game as a coherent whole.
