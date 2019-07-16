pipeline {
    agent any

    stages {
        stage('Build') { 
            steps { 
                echo "Build... $GIT_BRANCH"
                sh "bash ./build.sh";
            }
        }
        stage('Deploy') {
            when {
                 expression { return "$GIT_BRANCH".startsWith("refs/tags/") }
            }
            steps {
                echo 'Deploying.... $NuGetKey'
            }
        }
    }
}
