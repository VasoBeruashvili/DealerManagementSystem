var app = angular.module('profileInfo-app', ['ui.bootstrap']);

app.controller('profileInfo-ctrl', function ($scope, $modal) {
    $('#overlay').hide();    

    $scope.openPasswordChangeModal = function () {
        var modalInstance = $modal.open({
            animation: true,
            templateUrl: 'PasswordChangeModal.html',
            controller: 'PasswordChangeModalController',
            size: 'md',
            backdrop: 'static'
        });
    }
});

app.controller('PasswordChangeModalController', function ($scope, $modalInstance, $http) {
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.validate = function () {
        var result = true;

        result = result && !isUndefinedNullOrEmpty($scope.oldPwd);
        if (!result) {
            $('#oldPwd').css('borderColor', '#FF2000');
        } else {
            $('#oldPwd').css('borderColor', '#CCC');
        }

        result = result && !isUndefinedNullOrEmpty($scope.newPwd);
        if (!result) {
            $('#newPwd').css('borderColor', '#FF2000');
        } else {
            $('#newPwd').css('borderColor', '#CCC');
        }

        result = result && !isUndefinedNullOrEmpty($scope.repeatPwd);
        if (!result) {
            $('#repeatPwd').css('borderColor', '#FF2000');
        } else {
            $('#repeatPwd').css('borderColor', '#CCC');
        }

        return result;
    }

    $scope.save = function () {
        if ($scope.validate()) {
            if ($scope.oldPwd !== $scope.newPwd) {
                if ($scope.newPwd === $scope.repeatPwd) {
                    $http.post('/home/changePassword', { oldPwd: $scope.oldPwd, newPwd: $scope.newPwd }).then(function (resp) {
                        if (resp.data) {
                            window.location.href = '/account/logout';
                        } else {
                            if (currentLanguage === 'ka-GE') {
                                alert('პაროლის შეცვლა ვერ მოხერხდა!');
                            }

                            if (currentLanguage === 'en-US') {
                                alert('Password change failed!');
                            }
                        }
                    });
                } else {
                    if (currentLanguage === 'ka-GE') {
                        alert('ახალი პაროლი არ ემთხვევა განმეორებითს!');
                    }

                    if (currentLanguage === 'en-US') {
                        alert('The new password does not match repeated!');
                    }
                }
            } else {
                if (currentLanguage === 'ka-GE') {
                    alert('ძველი და ახალი პაროლები ემთხვევა ერთმანეთს!');
                }

                if (currentLanguage === 'en-US') {
                    alert('Old and new passwords match each other!');
                }
            }
        }
    }
});