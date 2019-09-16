// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const app = angular.module("myApp", []);

app.controller("myCtrl", function ($scope, $http) {

    let allRepository;
    $scope.showRepository = false;

    // return all github repository filter by user searched
    $scope.searchGithubRepository = () => {
        $("#overlay").fadeIn(300); // show the loading gif
        

        // get exist repository
        $http({
            method: "GET",
            url: "Home/GetSavedRepositoryIds"
        })
            .then((response) => {
                const savedRepositoriesIds = response.data;

                callGithubApi(savedRepositoriesIds);
            });
    }

    function callGithubApi(savedRepositoriesIds) {
        let arrRepositories = new Array();
        let isExistSavedRepository = false;
        if (savedRepositoriesIds.length > 0) {
            isExistSavedRepository = true;
        }
        $http({
            method: "GET",
            url: 'https://api.github.com/search/repositories?q=' + $scope.userSearch
        })
            .then(
                function mySuccess(response) {
                    allRepository = response.data.items;
                    for (let i = 0; i < allRepository.length; i++) {

                        let isSavedRepository = "false";
                        if (isExistSavedRepository === true) {
                            if (savedRepositoriesIds.includes(allRepository[i].id)) {
                                isSavedRepository = "true";
                            }
                        }
                        arrRepositories.push({
                            "id": allRepository[i].id,
                            "name": allRepository[i].name,
                            "avatar": allRepository[i].owner.avatar_url,
                            "isSavedRepository": isSavedRepository
                        });
                    }
                    $scope.allGithubRepository = arrRepositories;

                    setTimeout(() => {
                        $("#overlay").fadeOut(300); // hide the loading gif
                        $scope.showRepository = true;
                    }, 500);
                },
                function myError(response) {
                    $scope.errorMessage = response.statusText;
                    $("#overlay").fadeOut(300); // hide the loading gif
                    $scope.showRepository = true;
                });   
    }
    // change the favorite icon
    $scope.changeFavorite = ($event, repoPassed) => {
        $event.target.classList.toggle("checked");
        let repo = {
            Id: repoPassed.id,
            Name: repoPassed.name,
            ImagePath: repoPassed.avatar
        }

        $http({
            method: "post",
            url: `Home/SaveFavoriteRepository?repo=${JSON.stringify(repo)}`

        })
        .then(function success(response) {
        },
        function error(error) {
            console.log(error)
        })
    }
});


