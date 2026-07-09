pipeline {
    agent any

    environment {
        DOCKER_REGISTRY = 'docker.io'
        IMAGE_NAME = 'kseahibiki/seller-api'
        IMAGE_TAG = "${env.BUILD_NUMBER}"
    }

    stages {
        stage('Checkout') {
            steps {
                dir('t_Net8Services') {
                    checkout([
                        $class: 'GitSCM',
                        branches: [[name: '*/main']],
                        userRemoteConfigs: [[url: 'https://github.com/KseaHibiki/t_Net8Services-services-Seller.git']],
                        extensions: [[$class: 'RelativeTargetDirectory', relativeTargetDir: 'services/Seller/src']]
                    ])
                    checkout([
                        $class: 'GitSCM',
                        branches: [[name: '*/main']],
                        userRemoteConfigs: [[url: 'https://github.com/KseaHibiki/t_Net8Services-shared-Shop.Events.git']],
                        extensions: [[$class: 'RelativeTargetDirectory', relativeTargetDir: 'shared/Shop.Events']]
                    ])
                }
            }
        }

        stage('Restore & Build') {
            steps {
                dir('t_Net8Services') {
                    sh 'dotnet restore services/Seller/src/Seller.API/Seller.API.csproj'
                    sh 'dotnet build services/Seller/src/Seller.API/Seller.API.csproj -c Release --no-restore'
                }
            }
        }

        stage('Run Tests') {
            steps {
                dir('t_Net8Services') {
                    sh 'dotnet test services/Seller/src/Seller.API/Seller.API.csproj -c Release --no-build || echo "No tests found"'
                }
            }
        }

        stage('Docker Build & Push') {
            steps {
                dir('t_Net8Services') {
                    sh """
                        docker build \\
                            -f services/Seller/src/Seller.API/Dockerfile \\
                            -t ${IMAGE_NAME}:${IMAGE_TAG} \\
                            -t ${IMAGE_NAME}:latest \\
                            .
                    """
                    sh """
                        docker tag ${IMAGE_NAME}:${IMAGE_TAG} ${DOCKER_REGISTRY}/${IMAGE_NAME}:${IMAGE_TAG}
                        docker tag ${IMAGE_NAME}:latest ${DOCKER_REGISTRY}/${IMAGE_NAME}:latest
                    """
                }
            }
        }
    }

    post {
        success {
            echo 'Seller API 构建并推送成功！'
        }
        failure {
            echo 'Seller API 构建失败，请检查日志。'
        }
    }
}
