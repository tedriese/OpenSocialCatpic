/**
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

/**
 * @fileoverview Defines a basic generic data instance, for unknown containers.
 * Containers can extend this to provide their users list as well as environment
 * specific info.
 */

TestData.DefaultData = function(containerName) {
  this.users = {};
  this.fields = new TestData.Fields();
  this.environment = new TestData.Environment();
  this.container = new TestData.Container(containerName);
}
