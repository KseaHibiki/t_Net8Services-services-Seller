pipeline {
    agent any

    environment {
        REGISTRY = 'localhost:5005'
        IMAGE_NAME = 'seller-api'
        IMAGE_TAG = "${env.BUILD_NUMBER}"
    }

    stages {
        stage('Checkout') {
            steps {
                dir('t_Net8Services') {
                    checkout([
                        $class: 'GitSCM',
                        branches: [[name: '*/main']],
                        userRemoteConfigs: [[
                            url: 'https://github.com/KseaHibiki/t_Net8Services-services-Seller.git',
                            credentialsId: 'github-pat-token'
                        ]],
                        extensions: [[$class: 'RelativeTargetDirectory', relativeTargetDir: 'services/Seller/src']]
                    ])
                    checkout([
                        $class: 'GitSCM',
                        branches: [[name: '*/main']],
                        userRemoteConfigs: [[
                            url: 'https://github.com/KseaHibiki/t_Net8Services-shared-Shop.Events.git',
                            credentialsId: 'github-pat-token'
                        ]],
                        extensions: [[$class: 'RelativeTargetDirectory', relativeTargetDir: 'shared/Shop.Events']]
                    ])
                }
            }
        }

        stage('Restore & Build') {
            steps {
                dir('t_Net8Services') {
                    bat 'dotnet restore services/Seller/src/Seller.API/Seller.API.csproj'
                    bat 'dotnet build services/Seller/src/Seller.API/Seller.API.csproj -c Release --no-restore'
                }
            }
        }

        stage('Docker Build & Push') {
            steps {
                dir('t_Net8Services') {
                    bat "docker build -f services/Seller/src/Seller.API/Dockerfile -t ${REGISTRY}/${IMAGE_NAME}:%IMAGE_TAG% -t ${REGISTRY}/${IMAGE_NAME}:latest ."
                    bat "docker push ${REGISTRY}/${IMAGE_NAME}:%IMAGE_TAG%"
                    bat "docker push ${REGISTRY}/${IMAGE_NAME}:latest"
                }
            }
        }
    }

    post {
        success {
            echo "✅ Seller API 镜像已推送: ${REGISTRY}/${IMAGE_NAME}:${IMAGE_TAG}"
        }
        failure {
            echo '❌ Seller API 构建失败'
        }
        always {
            cleanWs()
        }
    }
}